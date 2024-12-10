# Microsoft.Extensions.AI.Evaluation - Samples

Microsoft.Extensions.AI.Evaluation is a set of .NET libraries (defined in the following NuGet packages) that provide
the tooling necessary to evaluate the quality and efficacy of LLM responses in your intelligent applications.

- [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation):
  Defines the core abstractions and types for supporting evaluation.
- [Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality):
  Contains evaluators that can be used to evaluate the quality of LLM responses in your applications including Relevance,
  Truth, Completeness, Fluency, Coherence, Equivalence and Groundedness.
- [Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting):
  Contains support for caching LLM responses, storing the results of evaluations and generating reports from the stored
  data.
- [Microsoft.Extensions.AI.Evaluation.Console](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Console):
  A command line dotnet tool for generating reports and managing evaluation data.

The Microsoft.Extensions.AI.Evaluation libraries are built on top of core AI abstractions defined in the
Microsoft.Extensions.AI libraries. Samples for Microsoft.Extensions.AI are available
[here](../microsoft-extensions-ai/README.md).

For more details, see the following blog post:
[Evaluate the quality of your AI applications with ease](https://devblogs.microsoft.com/dotnet/evaluate-the-quality-of-your-ai-applications-with-ease/)

## Samples

| Name | Description |
| --- | --- |
| [API Usage Examples](./api/README.md) | Examples that demonstrate the various APIs that are included in the above NuGet packages. |
