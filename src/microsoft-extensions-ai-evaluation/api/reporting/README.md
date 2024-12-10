# Microsoft.Extensions.AI.Evaluation - Reporting API Examples

This project contains a set of examples that demonstrate how concepts that are included in the
[Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation) and
[Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality)
NuGet packages can be used in conjunction with concepts (such as response caching, result storage and report
generation) that are available in the
[Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting)
NuGet package.

The examples in this project demonstrate how to orchestrate 'offline' evaluation by leveraging unit testing frameworks,
the test runners available within IDEs (to perform evaluations locally as part of your development inner loop) and
tools such as `dotnet test` (to perform evaluations as part of your CI/CD pipelines).

These examples also demonstrate how you can improve performance, and control the costs incurred, when using LLMs to
perform frequent offline evaluations (in your CI\CD pipelines, for example). By leveraging the response caching
functionality available in the
[Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting)
NuGet package, you can store previously fetched LLM responses in an (on-prem) cache, and use the cached LLM responses
for subsequent requests, as long as the request parameters (such as model and endpoint invoked, request prompts,
included context, etc.) remain unchanged.

Finally, these examples also demonstrate how the reporting functionality can be leveraged to store evaluation results
(across multiple executions over time), and generate reports based on the stored results.

**Note:** Because the examples included in this project leverage the response caching functionality, the corresponding
unit tests will invoke the LLM only the first time they are executed. The corresponding LLM responses will be cached
during this first run. Subsequent runs of the same test will fetch LLM responses from the cache and therefore execute
faster (as long as the request parameters remain unchanged). Note that by default, cached responses would expire after
14 days. When you run the examples, any cached responses that have expired will be automatically refreshed by fetching
a new response from the LLM.

## Sample Structure

All examples in this project are structured as unit tests, and each unit test is a self-contained example that
demonstrates a specific concept or API.

Each unit test is defined in its own separate .cs file. However, in order to make it easier to share the same test
setup code across multiple unit tests, all tests are defined within the same (partial) test class.

## Running the examples

See [INSTRUCTIONS.md](../INSTRUCTIONS.md)

## Samples

| Name | Description |
| --- | --- |
| [ReportingExamples.cs](./ReportingExamples.cs) | Contains setup code that is shared by all unit tests in the project. |
| [ReportingExamples.Example01_SamplingAndEvaluatingSingleResponse.cs](./ReportingExamples.Example01_SamplingAndEvaluatingSingleResponse.cs) | Demonstrates how to evaluate a single LLM response for a given question. |
| [ReportingExamples.Example02_SamplingAndEvaluatingMultipleResponses.cs](./ReportingExamples.Example02_SamplingAndEvaluatingMultipleResponses.cs) | Demonstrates how to sample multiple LLM responses for a given question and evaluate each of the sampled responses. |
| [ReportingExamples.Example03_SamplingAndEvaluatingMultipleResponsesInParallel.cs](./ReportingExamples.Example03_SamplingAndEvaluatingMultipleResponsesInParallel.cs) | Demonstrates how to sample multiple LLM responses for a given question and evaluate each of the sampled responses in parallel. |
| [ReportingExamples.Example04_DisablingResponseCaching.cs](./ReportingExamples.Example04_DisablingResponseCaching.cs) | Demonstrates how to turn off response caching.<br><br>Notice that unlike other unit tests in the project which execute slowly the first time around and are fast in subsequent exections, this test takes roughly the same amount of time to execute each time. |
| [ReportingExamples.Example05_InvokingEvaluatorsThatNeedAdditionalContext.cs](./ReportingExamples.Example05_InvokingEvaluatorsThatNeedAdditionalContext.cs) | Demonstrates how to invoke evaluators that need some additional context (such as grounding context in the case of the Groundedness evaluator or baseline ground truth for the Equivalence evaluator). |
| [ReportingExamples.Example06_AttachingDiagnosticsToMetrics.cs](./ReportingExamples.Example06_AttachingDiagnosticsToMetrics.cs) | Demonstrates how to log diagnostics to evaluation metrics included in the evaluation result.<br><br>Inspect the generated report at the end to understand how diagnostics are reported. |
| [ReportingExamples.Example07_ChangingInterpretationOfMetrics.cs](./ReportingExamples.Example07_ChangingInterpretationOfMetrics.cs) | Demonstrates how the interpretation of evaluation metrics included in the evaluation result can be changed.<br><br>Inspect the generated report at the end to understand how the changed interpretation is displayed in the report. |
| [ReportingExamples.Example08_UsingCustomStorage_01.cs](./ReportingExamples.Example08_UsingCustomStorage_01.cs) | Demonstrates how to use custom (SQLite-based) storage providers to store evaluation results and to cache LLM responses. |
| [ReportingExamples.Example09_UsingCustomStorage_02.cs](./ReportingExamples.Example09_UsingCustomStorage_02.cs) | Another example that demonstrates how to use custom (SQLite-based) storage providers to store evaluation results and to cache LLM responses. |
| [SqliteResponseCache.cs](./Storage/Sqlite/SqliteResponseCache.cs)<br>[SqliteResponseCache.Provider.cs](./Storage/Sqlite/SqliteResponseCache.Provider.cs) | Demonstrates how to author a custom (SQLite-based) storage provider to cache LLM responses. |
| [SqliteResultStore.cs](./Storage/Sqlite/SqliteResultStore.cs) | Demonstrates how to author a custom (SQLite-based) storage provider to store evaluation result data. |
| [ReportingExamples.Example10_GeneratingReportProgrammatically.cs](./ReportingExamples.Example10_GeneratingReportProgrammatically.cs) | Demonstrates how to generate a report for the most recent execution of the examples above that store their evaluation results on disk (using `DiskBasedReportingConfiguration` and `DiskBasedResultStore`). |
| [ReportingExamples.Example11_GeneratingReportProgrammaticallyFromCustomStorage.cs](./ReportingExamples.Example11_GeneratingReportProgrammaticallyFromCustomStorage.cs) | Demonstrates how to generate a report for the most recent execution of the examples above that store their evaluation results in a custom (SQLite-based) result store. |
