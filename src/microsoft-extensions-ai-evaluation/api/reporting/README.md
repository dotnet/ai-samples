---
page_type: sample
languages:
- csharp
products:
- dotnet
name: Evaluation with reporting examples
urlFragment: meai-evaluation-reporting
---

# Microsoft.Extensions.AI.Evaluation - Reporting API Examples

This project contains a set of examples that demonstrate how concepts and evaluators that are included in the
[Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation),
[Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality),
and [Microsoft.Extensions.AI.Evaluation.Safety](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Safety)
NuGet packages can be used in conjunction with concepts (such as response caching, result storage, and report
generation) that are available in the
[Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting),
and [Microsoft.Extensions.AI.Evaluation.Reporting.Azure](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting.Azure)
NuGet packages.

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
| [ReportingExamples.Example04_DisablingResponseCaching.cs](./ReportingExamples.Example04_DisablingResponseCaching.cs) | Demonstrates how to turn off response caching.<br><br>Notice that unlike other unit tests in the project which execute slowly the first time around and are fast in subsequent executions, this test takes roughly the same amount of time to execute each time. |
| [ReportingExamples.Example05_InvokingEvaluatorsThatNeedAdditionalContext.cs](./ReportingExamples.Example05_InvokingEvaluatorsThatNeedAdditionalContext.cs) | Demonstrates how to invoke evaluators that need some additional context (such as grounding context in the case of the Groundedness evaluator or baseline ground truth for the Equivalence evaluator). |
| [ReportingExamples.Example06_AttachingDiagnosticsToMetrics.cs](./ReportingExamples.Example06_AttachingDiagnosticsToMetrics.cs) | Demonstrates how to log diagnostics to evaluation metrics included in the evaluation result.<br><br>Inspect the generated report at the end to understand how diagnostics are reported. |
| [ReportingExamples.Example07_ChangingInterpretationOfMetrics.cs](./ReportingExamples.Example07_ChangingInterpretationOfMetrics.cs) | Demonstrates how the interpretation of evaluation metrics included in the evaluation result can be changed.<br><br>Inspect the generated report at the end to understand how the changed interpretation is displayed in the report. |
| [ReportingExamples.Example08_RunningSafetyEvaluators.cs](./ReportingExamples.Example08_RunningSafetyEvaluators.cs) | Demonstrates how to run safety evaluators that leverage the Azure AI Foundry Evaluation service to evaluate content harm and safety metrics.<br><br>See [Running content safety evaluation examples](../INSTRUCTIONS.md#running-content-safety-evaluation-examples) for instructions to configure the Azure AI Foundry Evaluation service for this example.<br><br>Inspect the generated report at the end and click on each of the metrics under this example to view additional details for each metric. |
| [ReportingExamples.Example09_RunningSafetyEvaluatorsAgainstResponsesWithImages.cs](./ReportingExamples.Example09_RunningSafetyEvaluatorsAgainstResponsesWithImages.cs) | Demonstrates how to run safety evaluators that leverage the Azure AI Foundry Evaluation service to evaluate content harm and safety metrics for responses that contain images.<br><br>See [Running content safety evaluation examples](../INSTRUCTIONS.md#running-content-safety-evaluation-examples) for instructions to configure the Azure AI Foundry Evaluation service for this example.<br><br>Inspect the generated report at the end and click on each of the metrics under this example to view additional details for each metric. |
| [ReportingExamples.Example10_RunningQualityAndSafetyEvaluatorsTogether.cs](./ReportingExamples.Example10_RunningQualityAndSafetyEvaluatorsTogether.cs) | Demonstrates how to run (LLM-based) quality evaluators and safety evaluators (that leverage the Azure AI Foundry Evaluation service) as part of the same `ReportingConfiguration`.<br><br>See [Running content safety evaluation examples](../INSTRUCTIONS.md#running-content-safety-evaluation-examples) for instructions to configure the Azure AI Foundry Evaluation service for this example. |
| [ReportingExamples.Example11_RunningAgentQualityEvaluators.cs](./ReportingExamples.Example11_RunningAgentQualityEvaluators.cs) | Demonstrates how to use agent quality evaluators to evaluate tool-based agentic workflows. |
| [ReportingExamples.Example12_RunningNLPEvaluators.cs](./ReportingExamples.Example12_RunningNLPEvaluators.cs) | Demonstrates how to use the NLP evaluators to compute text similarity scores between AI responses and supplied reference text.<br><br>Note that unlike the evaluators demonstrated in other examples, the NLP evaluators do not require an LLM to perform the evaluation. Instead, they use traditional NLP techniques (text tokenization, n-gram analysis, etc.) to compute text similarity scores. |
| [ReportingExamples.Example13_UsingAzureStorage_01.cs](./ReportingExamples.Example13_UsingAzureStorage_01.cs) | Demonstrates how to use Azure storage providers to store evaluation results and to cache LLM responses in an Azure storage container.<br><br>See [Running Azure storage examples](../INSTRUCTIONS.md#running-azure-storage-examples) for instructions to configure Azure storage for this example. |
| [ReportingExamples.Example14_UsingAzureStorage_02.cs](./ReportingExamples.Example14_UsingAzureStorage_02.cs) | Another example that demonstrates how to Azure storage providers to store evaluation results and to cache LLM responses in an Azure storage container.<br><br>See [Running Azure storage examples](../INSTRUCTIONS.md#running-azure-storage-examples) for instructions to configure Azure storage for this example. |
| [ReportingExamples.Example15_UsingCustomStorage_01.cs](./ReportingExamples.Example15_UsingCustomStorage_01.cs) | Demonstrates how to use custom (SQLite-based) storage providers to store evaluation results and to cache LLM responses. |
| [ReportingExamples.Example16_UsingCustomStorage_02.cs](./ReportingExamples.Example16_UsingCustomStorage_02.cs) | Another example that demonstrates how to use custom (SQLite-based) storage providers to store evaluation results and to cache LLM responses. |
| [ReportingExamples.Example17_GeneratingReportProgrammatically.cs](./ReportingExamples.Example17_GeneratingReportProgrammatically.cs) | Demonstrates how to generate a report for the most recent execution of the examples above that store their evaluation results on disk (using `DiskBasedReportingConfiguration` and `DiskBasedResultStore`). |
| [ReportingExamples.Example18_GeneratingReportProgrammaticallyFromAzureStorage.cs](./ReportingExamples.Example18_GeneratingReportProgrammaticallyFromAzureStorage.cs) | Demonstrates how to generate a report for the most recent execution of the examples above that store their evaluation results in an Azure storage container. |
| [ReportingExamples.Example19_GeneratingReportProgrammaticallyFromCustomStorage.cs](./ReportingExamples.Example19_GeneratingReportProgrammaticallyFromCustomStorage.cs) | Demonstrates how to generate a report for the most recent execution of the examples above that store their evaluation results in a custom (SQLite-based) result store. |
| [SqliteResponseCache.cs](./Storage/Sqlite/SqliteResponseCache.cs)<br>[SqliteResponseCache.Provider.cs](./Storage/Sqlite/SqliteResponseCache.Provider.cs) | Demonstrates how to author a custom (SQLite-based) storage provider to cache LLM responses. |
| [SqliteResultStore.cs](./Storage/Sqlite/SqliteResultStore.cs) | Demonstrates how to author a custom (SQLite-based) storage provider to store evaluation result data. |
