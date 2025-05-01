// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;

namespace Reporting;

public partial class ReportingExamples
{
    [TestMethod]
    public async Task Example12_UsingAzureStorage_02()
    {
        /// This test requires additional environment variables to be set in order to use Azure storage. The test is
        /// skipped if these environment variables are not set.
        SkipTestIfAzureStorageNotConfigured();

        /// This is another example similar to the previous one in <see cref="Example11_UsingAzureStorage_01"/> that
        /// stores evaluation results and caches LLM responses in the same Azure Storage container defined in
        /// <see cref="s_azureStorageReportingConfiguration"/>.

        await using ScenarioRun scenarioRun =
            await s_azureStorageReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName, additionalTags: ["Jupiter"]);

        (IList<ChatMessage> messages, ChatResponse modelResponse) =
            await GetAstronomyConversationAsync(
                chatClient: scenarioRun.ChatConfiguration!.ChatClient,
                astronomyQuestion: "How far is the planet Jupiter from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        Validate(result);

        /// At this point, the <see cref="scenarioRun"/> object will be disposed and the evaluation result for the
        /// above evaluation will be stored to the result store in Azure Storage that is configured in
        /// <see cref="s_azureStorageReportingConfiguration"/>.
    }
}
