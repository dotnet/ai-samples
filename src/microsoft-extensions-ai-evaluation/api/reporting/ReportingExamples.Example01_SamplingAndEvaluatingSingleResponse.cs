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
    public async Task Example01_SamplingAndEvaluatingSingleResponse()
    {
        /// Use <see cref="s_defaultReportingConfiguration"/> to create a <see cref="ScenarioRun"/> with
        /// <see cref="ScenarioRun.ScenarioName"/> set to the fully qualified name of the current test method.
        await using ScenarioRun scenarioRun =
            await s_defaultReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName);

        /// Get an LLM response to be evaluated. Note that we use the <see cref="IChatClient"/> that is included in the
        /// <see cref="ScenarioRun.ChatConfiguration"/> to get this response. Since the <see cref="scenarioRun"/> was
        /// created using <see cref="s_defaultReportingConfiguration"/>, and since response caching is turned on for
        /// <see cref="s_defaultReportingConfiguration"/>, the LLM response fetched using the <see cref="IChatClient"/>
        /// included in <see cref="ScenarioRun.ChatConfiguration"/> will be fetched:
        /// - directly from the LLM endpoint that was configured in
        ///   <see cref="s_defaultReportingConfiguration"/> in the very first run of the current test
        /// - from the (disk-based) response cache that was configured in <see cref="s_defaultReportingConfiguration"/>
        ///   in every subsequent run of the test, until the cached entry expires (in 14 days by default) and ends up
        ///   being refreshed
        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the Moon from the Earth at its closest and furthest points?");

        /// Run the evaluators configured in <see cref="s_defaultReportingConfiguration"/> against the response. Since
        /// the <see cref="scenarioRun"/> was created using <see cref="s_defaultReportingConfiguration"/>, and since
        /// response caching is turned on for <see cref="s_defaultReportingConfiguration"/>, the evaluation will be:
        /// - performed using the the LLM endpoint that was configured in
        ///   <see cref="s_defaultReportingConfiguration"/> in the very first run of the current test
        /// - fetched from the (disk-based) response cache that was configured in
        ///   <see cref="s_defaultReportingConfiguration"/> in every subsequent run of the test, until the cached entry
        ///   expires (in 14 days by default) and ends up being refreshed
        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        /// Run some basic validation on the evaluation result.
        /// 
        /// Note: This step is optional and mainly for demonstration purposes. In real-world evaluations, you may not
        /// want to validate individual results since the LLM responses and evaluation scores can jump around a bit
        /// over time as your product (and the models used) evolve. You may not want individual evaluation tests to
        /// 'fail' and block builds in your CI/CD pipelines when this happens. Instead, in such cases, it may be better
        /// to rely on the generated report and track the overall trends for evaluation scores across different
        /// scenarios over time (and only fail individual builds in your CI/CD pipelines when there is a significant
        /// drop in evaluation scores across multiple different tests). That said, there is some nuance here and the
        /// choice of whether to validate individual results or not can vary depending on the specific use case.
        Validate(result);

        /// At this point, the <see cref="scenarioRun"/> object will be disposed and the evaluation result for the
        /// above evaluation will be stored to the (disk-based) result store that is configured in
        /// <see cref="s_defaultReportingConfiguration"/>. You can inspect how the result is stored by navigating to the
        /// directory that you specified via the 'EVAL_SAMPLE_STORAGE_ROOT_PATH' environment variable.
    }
}
