// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;

namespace Reporting;

public partial class ReportingExamples
{
    [TestMethod]
    public async Task Example09_UsingCustomStorage_02()
    {
        /// This is another example similar to the previous one in <see cref="Example08_UsingCustomStorage_01"/> that
        /// stores evaluation results and caches LLM responses to the same SQLite databases defined in
        /// <see cref="s_sqliteReportingConfiguration"/>.

        await using ScenarioRun scenarioRun =
            await s_sqliteReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the dwarf planet Pluto from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        Validate(result);
    }
}
