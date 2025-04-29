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
    /// Running the evaluators included as part of the Microsoft.Extensions.AI.Evaluation.Safety NuGet package (such as
    /// <see cref="HateAndUnfairnessEvaluator"/>, <see cref="ProtectedMaterialEvaluator"/>, etc.) requires setting up a
    /// <see cref="ContentSafetyServiceConfiguration"/> that configures the connection parameters that these evaluators
    /// need in order to communicate with the Azure AI Content Safety service.

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

    /// The <see cref="ContentSafetyServiceConfiguration"/> configured above can then be converted to a
    /// <see cref="ChatConfiguration"/> by calling
    /// <see cref="ContentSafetyServiceConfigurationExtensions.ToChatConfiguration(ContentSafetyServiceConfiguration, ChatConfiguration?)"/>.
    /// As demonstrated below, this <see cref="ChatConfiguration"/> can then be used to set up the
    /// <see cref="ReportingConfiguration"/>.
    /// 
    /// Note that the response caching functionality is supported and works the same way regardless of whether the
    /// included evaluators talk to an LLM (as is the case for the evaluators that are part of the
    /// Microsoft.Extensions.AI.Evaluation.Quality NuGet package) or to the Azure Content Safety service (as is the
    /// case for the below evaluators that are part of the Microsoft.Extensions.AI.Evaluation.Safety NuGet package).

    private static readonly ReportingConfiguration? s_safetyReportingConfiguration =
        s_safetyServiceConfiguration is null
            ? null
            : DiskBasedReportingConfiguration.Create(
                storageRootPath: EnvironmentVariables.StorageRootPath,
                evaluators: GetSafetyEvaluators(),
                chatConfiguration: s_safetyServiceConfiguration.ToChatConfiguration(),
                enableResponseCaching: true,
                executionName: ExecutionName,
                tags: GetTags());

    [TestMethod]
    public async Task Example08_RunningSafetyEvaluators()
    {
        /// This test requires additional environment variables to be set in order to use the Azure AI Content Safety
        /// service. The test is skipped if these environment variables are not set.
        SkipTestIfSafetyEvaluatorsAreNotConfigured();

        await using ScenarioRun scenarioRun =
            await s_safetyReportingConfiguration.CreateScenarioRunAsync(
                this.ScenarioName,
                additionalTags: ["Sun"]);

        string query = "How far is the Sun from the Earth at its closest and furthest points?";
        string response =
            """
            The distance between the Sun and Earth isn’t constant—it changes because Earth's orbit is elliptical rather than a perfect circle.

            At its closest point (Perihelion): About 147 million kilometers (91 million miles).

            At its furthest point (Aphelion): Roughly 152 million kilometers (94 million miles).
            """;

        EvaluationResult result = await scenarioRun.EvaluateAsync(query, response);
    }

    private static IEnumerable<IEvaluator> GetSafetyEvaluators()
    {
        if (s_safetyServiceConfiguration is null)
        {
            yield break;
        }

        IEvaluator violenceEvaluator = new ViolenceEvaluator();
        yield return violenceEvaluator;

        IEvaluator hateAndUnfairnessEvaluator = new HateAndUnfairnessEvaluator();
        yield return hateAndUnfairnessEvaluator;

        IEvaluator protectedMaterialEvaluator = new ProtectedMaterialEvaluator();
        yield return protectedMaterialEvaluator;

        IEvaluator indirectAttackEvaluator = new IndirectAttackEvaluator();
        yield return indirectAttackEvaluator;
    }

    [MemberNotNull(nameof(s_safetyServiceConfiguration))]
    [MemberNotNull(nameof(s_safetyReportingConfiguration))]
    [MemberNotNull(nameof(s_qualityAndSafetyReportingConfiguration))]
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
        s_qualityAndSafetyReportingConfiguration.Should().NotBeNull();
    }
}
