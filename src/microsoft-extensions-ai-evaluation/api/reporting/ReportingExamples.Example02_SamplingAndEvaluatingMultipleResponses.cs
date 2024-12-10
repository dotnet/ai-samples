// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;

namespace Reporting;

public partial class ReportingExamples
{
    private static IEnumerable<object[]> Iterations
    {
        get
        {
            for (int i = 1; i <= 3; ++i)
            {
                yield return new object[] { i.ToString() };
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(Iterations))]
    public async Task Example02_SamplingAndEvaluatingMultipleResponses(string iterationName)
    {
        /// This example is similar to the previous one in
        /// <see cref="Example01_SamplingAndEvaluatingSingleResponse"/> with the only difference being that we sample
        /// and evaluate multiple responses in this example (instead of just one).

        /// Use <see cref="s_defaultReportingConfiguration"/> to create a <see cref="ScenarioRun"/> with
        /// <see cref="ScenarioRun.ScenarioName"/> set to the fully qualified name of the current test method and
        /// see <see cref="ScenarioRun.IterationName"/> set to the number representing the current iteration.
        await using ScenarioRun scenarioRun =
            await s_defaultReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName, iterationName);

        /// Get an LLM response to be evaluated for the current iteration. As previously explained in the comments in
        /// <see cref="Example01_SamplingAndEvaluatingSingleResponse"/>, the response to be evaluated will be fetched
        /// directly from the LLM in the very first run of each individual iteration, and from the (disk-based)
        /// response cache in every subsequent run of the same iteration until the cached entries expire (in 14 days
        /// by default).
        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the planet Jupiter from the Earth at its closest and furthest points?");

        /// Run the evaluators configured in <see cref="s_defaultReportingConfiguration"/> against the response. Again,
        /// the evaluation will be performed using the the LLM in the very first run of each individual iteration, and
        /// fetched from the (disk-based) response cache in every subsequent run of the same iteration until the cached
        /// entries expire (in 14 days by default).
        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        /// Run some basic validation on the evaluation result.
        Validate(result);

        /// At this point, the <see cref="scenarioRun"/> object will be disposed and the evaluation result for the
        /// current iteration above will be stored to the (disk-based) result store. You can inspect how the result for
        /// each iteration is stored by navigating to the directory that you specified via the
        /// 'EVAL_SAMPLE_STORAGE_ROOT_PATH' environment variable.
    }
}
