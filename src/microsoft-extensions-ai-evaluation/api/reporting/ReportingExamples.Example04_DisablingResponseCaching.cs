// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Setup;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage.Disk;

namespace Reporting;

public partial class ReportingExamples
{
    private static readonly ReportingConfiguration s_reportingConfigurationWithCachingDisabled =
        DiskBasedReportingConfiguration.Create(
            storageRootPath: EnvironmentVariables.StorageRootPath,
            evaluators: GetEvaluators(),
            chatConfiguration: TestSetup.GetChatConfiguration(),
            enableResponseCaching: false,
            executionName: ExecutionName);

    [TestMethod]
    public async Task Example04_DisablingResponseCaching()
    {
        /// This example is similar to the one in
        /// <see cref="Example01_SamplingAndEvaluatingSingleResponse"/> with the only difference being that this
        /// example uses an alternate <see cref="ReportingConfiguration"/>
        /// (<see cref="s_reportingConfigurationWithCachingDisabled"/> above) to turn of LLM response caching.
        /// 
        /// This means that the this test will always invoke the LLM directly, both to fetch the response to be
        /// evaluated, as well as to perform the evaluation. Since this test will always invoke the LLM, subsequent
        /// runs of this test will be just as slow as the first run of this test (unlike the remaining tests in the
        /// current project, all of which leverage response caching to speed up subsequent runs).

        await using ScenarioRun scenarioRun =
            await s_reportingConfigurationWithCachingDisabled.CreateScenarioRunAsync(this.ScenarioName);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the planet Mercury from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        Validate(result);
    }
}
