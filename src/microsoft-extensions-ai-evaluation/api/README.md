# Microsoft.Extensions.AI.Evaluation - API Usage Examples

[Examples.sln](./Examples.sln) contains a set of examples that demonstrate various concepts and APIs that are included
in the following three NuGet packages:
- [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation):
  Defines the core abstractions and types for supporting evaluation.
- [Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality):
  Contains evaluators that can be used to evaluate the quality of LLM responses in your applications including Relevance,
  Truth, Completeness, Fluency, Coherence, Equivalence and Groundedness.
- [Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting):
  Contains support for caching LLM responses, storing the results of evaluations and generating reports from the stored
  data.

## Sample Structure

All examples in [Examples.sln](./Examples.sln) are structured as unit tests, and each unit test is a self-contained
example that demonstrates a specific concept or API.

Each unit test is defined in its own separate .cs file. However, in order to make it easier to share the same test
setup code across multiple unit tests, all tests in a given project are defined within the same (partial) test class.

The examples are split across the following two projects:

| Name | Description |
| --- | --- |
| [Evaluation API Examples](./evaluation/README.md) | <br>A set of examples that demonstrate core concepts (such as evaluators, evaluation results, diagnostics and result interpretation) that are included in the [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation) and [Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality) packages above.<br><br>The examples in this project demonstrate how to perform evaluations without bringing in any of the reporting functionality present in the [Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting) package. This can be useful in contexts where you either don't need the response caching, result storage and report generation functionality, or where you need to store and report the results in other ways. An example of this may be the case where you need to perform 'online' evaluation of LLM responses within your deployed production code, and report the evaluation results via your product's existing telemetry pipeline.<br><br>**Note:** The examples included in this project invoke the LLM to perform an evaluation each time the corresponding unit test is executed. These examples do not leverage the response caching functionality described below, and will therefore generally run slower than the examples included in the [Reporting API Examples](./reporting/README.md) below.<br><br> |
| [Reporting API Examples](./reporting/README.md) | <br>A set of examples that demonstrate how concepts that are included in the [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation) and [Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality) packages above can be used in conjunction with concepts (such as response caching, result storage and report generation) that are available in the [Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting) package.<br><br>The examples in this project demonstrate how to orchestrate 'offline' evaluation by leveraging unit testing frameworks, the test runners available within IDEs (to perform evaluations locally as part of your development inner loop) and tools such as `dotnet test` (to perform evaluations as part of your CI/CD pipelines).<br><br>These examples also demonstrate how you can improve performance, and control the costs incurred, when using LLMs to perform frequent offline evaluations (in your CI\CD pipelines, for example). By leveraging the response caching functionality available in the [Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting) package, you can store previously fetched LLM responses in an (on-prem) cache, and use the cached LLM responses for subsequent requests, as long as the request parameters (such as model and endpoint invoked, request prompts, included context, etc.) remain unchanged.<br><br>Finally, these examples also demonstrate how the reporting functionality can be leveraged to store evaluation results (across multiple executions over time), and generate reports based on the stored results.<br><br> **Note:** Because the examples included in this project leverage the response caching functionality, the corresponding unit tests will invoke the LLM only the first time they are executed. The corresponding LLM responses will be cached during this first run. Subsequent runs of the same test will fetch LLM responses from the cache and therefore execute faster (as long as the request parameters remain unchanged). Note that by default, cached responses would expire after 14 days. When you run the examples, any cached responses that have expired will be automatically refreshed by fetching a new response from the LLM.<br><br> |

## Running the examples

See [INSTRUCTIONS.md](./INSTRUCTIONS.md)