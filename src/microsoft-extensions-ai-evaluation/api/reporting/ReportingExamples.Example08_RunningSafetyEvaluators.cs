// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Evaluation.Setup;
using FluentAssertions;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.AI.Evaluation.Safety;

namespace Reporting;

public partial class ReportingExamples
{
    private static readonly ContentSafetyServiceConfiguration? s_safetyServiceConfiguration =
        EnvironmentVariables.AzureSubscriptionId is null ||
        EnvironmentVariables.AzureResourceGroup is null ||
        EnvironmentVariables.AzureAIProject is null
            ? null
            : new ContentSafetyServiceConfiguration(
                credential: new DefaultAzureCredential(),
                subscriptionId: EnvironmentVariables.AzureSubscriptionId,
                resourceGroupName: EnvironmentVariables.AzureResourceGroup,
                projectName: EnvironmentVariables.AzureAIProject);

    private static readonly ReportingConfiguration? s_safetyReportingConfiguration =
        s_safetyServiceConfiguration is null
            ? null
            : DiskBasedReportingConfiguration.Create(
                storageRootPath: EnvironmentVariables.StorageRootPath,
                evaluators: GetSafetyEvaluators(),
                chatConfiguration: s_chatConfiguration,
                enableResponseCaching: true,
                executionName: ExecutionName,
                tags: GetTags());

    [TestMethod]
    public async Task Example08_RunningSafetyEvaluators()
    {
        /// This test requires additional environment variables to be set in order to use the Azure AI Content Safety
        /// service. The test is skipped if these environment variables are not set.
        SkipTestIfSafetyEvaluatorsAreNotConfigured();

        /// Note that response caching does not work at the moment for the Content Safety evaluators in this example.
        /// This is a known issue and will be fixed in a future release.
        /// See https://github.com/dotnet/extensions/issues/6260.

        await using ScenarioRun scenarioRun =
            await s_safetyReportingConfiguration.CreateScenarioRunAsync(
                this.ScenarioName,
                additionalTags: ["Sun"]);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the Sun from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);
    }

    private static IEnumerable<IEvaluator> GetSafetyEvaluators()
    {
        if (s_safetyServiceConfiguration is null)
        {
            yield break;
        }

        IEvaluator violenceEvaluator = new ViolenceEvaluator(s_safetyServiceConfiguration);
        yield return violenceEvaluator;

        IEvaluator hateAndUnfairnessEvaluator = new HateAndUnfairnessEvaluator(s_safetyServiceConfiguration);
        yield return hateAndUnfairnessEvaluator;

        IEvaluator protectedMaterialEvaluator = new ProtectedMaterialEvaluator(s_safetyServiceConfiguration);
        yield return protectedMaterialEvaluator;

        IEvaluator indirectAttackEvaluator = new IndirectAttackEvaluator(s_safetyServiceConfiguration);
        yield return indirectAttackEvaluator;
    }

    [MemberNotNull(nameof(s_safetyServiceConfiguration))]
    [MemberNotNull(nameof(s_safetyReportingConfiguration))]
    private static void SkipTestIfSafetyEvaluatorsAreNotConfigured()
    {
        if (s_safetyReportingConfiguration is null)
        {
            Assert.Inconclusive(
                $"""
                The test was skipped since the following environment variables were not set. Set these variables to configure the Azure Content Safety service for use in this test:
                set {EnvironmentVariables.AzureSubscriptionIdVariableName}=<The subscription ID of the Azure account that contains the below Azure AI project>
                set {EnvironmentVariables.AzureResourceGroupVariableName}=<The name of the Azure resource group under the above subscription that contains the below Azure AI project>
                set {EnvironmentVariables.AzureAIProjectVariableName}=<The name of the Azure AI project under which the Content Safety evaluations are to be executed>
                """);
        }

        s_safetyServiceConfiguration.Should().NotBeNull();
        s_safetyReportingConfiguration.Should().NotBeNull();
    }
}
