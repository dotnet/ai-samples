// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Setup;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Reporting.Storage.Sqlite;

namespace Reporting;

public partial class ReportingExamples
{
    /// The SQLite .db files for the result store and the response cache are stored under the directory that you
    /// specified via the 'EVAL_SAMPLE_STORAGE_ROOT_PATH' environment variable.

    private static readonly string s_sqliteResultsFilePath =
        Path.Combine(EnvironmentVariables.StorageRootPath, "results.db");

    private static readonly string s_sqliteCacheFilePath =
        Path.Combine(EnvironmentVariables.StorageRootPath, "cache.db");

    private static readonly ReportingConfiguration s_sqliteReportingConfiguration =
        new ReportingConfiguration(
            evaluators: GetEvaluators(),
            resultStore: new SqliteResultStore(s_sqliteResultsFilePath),
            chatConfiguration: TestSetup.GetChatConfiguration(),
            responseCacheProvider: new SqliteResponseCache.Provider(s_sqliteCacheFilePath),
            executionName: ExecutionName);

    [TestMethod]
    public async Task Example08_UsingCustomStorage_01()
    {
        /// This example demonstrates how to create and use a custom <see cref="ReportingConfiguration"/>
        /// (see <see cref="s_sqliteReportingConfiguration"/> above) that uses SQLite databases for storing evaluation
        /// results and for caching LLM responses. The result store implementation is provided via
        /// <see cref="SqliteResultStore"/>, while the response caching implementation is provided via
        /// <see cref="SqliteResponseCache.Provider"/>. Both <see cref="SqliteResultStore"/> as well as
        /// <see cref="SqliteResponseCache.Provider"/> are defined within the current project.

        await using ScenarioRun scenarioRun =
            await s_sqliteReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the planet Saturn from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        Validate(result);

        /// At this point, the <see cref="scenarioRun"/> object will be disposed and the evaluation result for the
        /// above evaluation will be stored to the SQLite result store that is configured in
        /// <see cref="s_sqliteReportingConfiguration"/>.
    }
}
