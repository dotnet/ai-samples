// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;

namespace Reporting;

public partial class ReportingExamples
{
    [TestMethod]
    public async Task Example03_SamplingAndEvaluatingMultipleResponsesInParallel()
    {
        /// This example is similar to the previous one in
        /// <see cref="Example02_SamplingAndEvaluatingMultipleResponses(string)"/> with the only difference being that
        /// we sample and evaluate multiple responses in parallel.
        /// 
        /// This example demonstrates that the evaluations themselves as well as the response caching and result
        /// storage operations are all thread safe and work reliably regardless of whether evaluations are performed
        /// sequentially versus in parallel (as long as distinct <see cref="ScenarioRun.ScenarioName"/>s and
        /// <see cref="ScenarioRun.IterationName"/>s are specified for the different evaluations being performed in
        /// parallel).

        await Parallel.ForAsync(1, 4, async (i, _) =>
        {
            await using ScenarioRun scenarioRun =
                await s_defaultReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName, iterationName: i.ToString());

            var (messages, modelResponse) = await GetAstronomyConversationAsync(
                chatClient: scenarioRun.ChatConfiguration!.ChatClient,
                astronomyQuestion: "How far is the planet Mars from the Earth at its closest and furthest points?");

            EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

            Validate(result);
        });
    }
}
