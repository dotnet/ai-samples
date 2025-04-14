# Microsoft.Extensions.AI.Evaluation - Samples

Microsoft.Extensions.AI.Evaluation is a set of .NET libraries (defined in the following NuGet packages) that provide
the tooling necessary to evaluate the quality and efficacy of LLM responses in your intelligent applications.

* [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation):
  Defines core abstractions and types for supporting evaluation.
* [Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality):
  Contains evaluators that can be used to evaluate the quality of AI responses in your projects including Relevance,
  Truth, Completeness, Fluency, Coherence, Equivalence and Groundedness.
* [Microsoft.Extensions.AI.Evaluation.Safety](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Safety):
  Contains a set of evaluators that are built atop the Azure AI Content Safety service that can be used to evaluate the
  content safety of AI responses in your projects including Protected Material, Groundedness Pro, Ungrounded
  Attributes, Hate and Unfairness, Self Harm, Violence, Sexual, Code Vulnerability and Indirect Attack.
* [Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting):
  Contains support for caching LLM responses, storing the results of evaluations and generating reports from that data.
* [Microsoft.Extensions.AI.Evaluation.Reporting.Azure](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting.Azure):
  Supports the `Microsoft.Extensions.AI.Evaluation.Reporting` library with an implementation for caching LLM responses
  and storing the evaluation results in an Azure Storage container.
* [Microsoft.Extensions.AI.Evaluation.Console](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Console):
  A command line dotnet tool for generating reports and managing evaluation data.

The Microsoft.Extensions.AI.Evaluation libraries are built on top of core AI abstractions defined in the
Microsoft.Extensions.AI libraries. Samples for Microsoft.Extensions.AI are available
[here](../microsoft-extensions-ai/README.md).

## Samples

| Name | Description |
| --- | --- |
| [API Usage Examples](./api/README.md) | Examples that demonstrate the various APIs that are included in the above NuGet packages. |

## Learn More

Check out the following .NET Live talk on YouTube:
[![.NET AI Community Standup: Evaluate the Quality of Your AI Applications](https://img.youtube.com/vi/kFdUpu9TdlY/maxresdefault.jpg)](https://youtu.be/kFdUpu9TdlY)

Also check out the following blog posts:
- [Evaluate the quality of your AI applications with ease](https://devblogs.microsoft.com/dotnet/evaluate-the-quality-of-your-ai-applications-with-ease/)
- [Unlock new possibilities for AI Evaluations for .NET](https://devblogs.microsoft.com/dotnet/start-using-the-microsoft-ai-evaluations-library-today/)

Documentation for the libraries is available on
[Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/ai/conceptual/evaluation-libraries).

## Feedback & Contributing

We welcome your feedback and contributions! Head to the [dotnet/extensions](https://github.com/dotnet/extensions) repo
to provide feedback or contribute to the project. Existing issues for the Microsoft.Extensions.AI.Evaluation libraries
can be viewed
[here](https://github.com/dotnet/extensions/issues?q=is%3Aissue%20state%3Aopen%20label%3Aarea-ai-eval). We also welcome
feedback and contributions for the samples in the current repository.
