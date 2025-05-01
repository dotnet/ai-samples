// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Evaluation.Setup;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

namespace Reporting;

public partial class ReportingExamples
{
    private static readonly DataLakeDirectoryClient? s_dataLakeDirectoryClient =
        EnvironmentVariables.AzureStorageAccountEndpoint is null ||
        EnvironmentVariables.AzureStorageContainer is null
            ? null
            : new DataLakeDirectoryClient(
                new Uri(
                    baseUri: new Uri(EnvironmentVariables.AzureStorageAccountEndpoint),
                    relativeUri: EnvironmentVariables.AzureStorageContainer),
                credential: new DefaultAzureCredential());

    private static readonly ReportingConfiguration? s_azureStorageReportingConfiguration =
        s_dataLakeDirectoryClient is null
            ? null
            : AzureStorageReportingConfiguration.Create(
                client: s_dataLakeDirectoryClient,
                evaluators: GetEvaluators(),
                chatConfiguration: s_chatConfiguration,
                executionName: ExecutionName,
                tags: GetTags(storageKind: "Storage: Azure Storage"));

    [TestMethod]
    public async Task Example11_UsingAzureStorage_01()
    {
        /// This test requires additional environment variables to be set in order to use Azure storage. The test is
        /// skipped if these environment variables are not set.
        SkipTestIfAzureStorageNotConfigured();

        /// This example demonstrates how to create and use <see cref="AzureStorageReportingConfiguration"/> that uses
        /// an Azure Storage container for storing evaluation results and for caching LLM responses.

        await using ScenarioRun scenarioRun =
            await s_azureStorageReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName, additionalTags: ["Saturn"]);

        (IList<ChatMessage> messages, ChatResponse modelResponse) =
            await GetAstronomyConversationAsync(
                chatClient: scenarioRun.ChatConfiguration!.ChatClient,
                astronomyQuestion: "How far is the planet Saturn from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        Validate(result);

        /// At this point, the <see cref="scenarioRun"/> object will be disposed and the evaluation result for the
        /// above evaluation will be stored to the result store in Azure Storage that is configured in
        /// <see cref="s_azureStorageReportingConfiguration"/>.
    }

    [MemberNotNull(nameof(s_dataLakeDirectoryClient))]
    [MemberNotNull(nameof(s_azureStorageReportingConfiguration))]
    private static void SkipTestIfAzureStorageNotConfigured()
    {
        if (s_azureStorageReportingConfiguration is null)
        {
            Assert.Inconclusive(
                $"""
                The test was skipped since the following environment variables were not set. Set these variables to configure the Azure storage for use in this test:
                set {EnvironmentVariables.AzureStorageAccountEndpoint}=<The endpoint url that identifies the Azure Data Lake Gen2 enabled storage account that contains the below storage container>
                set {EnvironmentVariables.AzureStorageContainer}=<The name of the Azure storage container>
                """);
        }

        s_dataLakeDirectoryClient.Should().NotBeNull();
        s_azureStorageReportingConfiguration.Should().NotBeNull();
    }
}
