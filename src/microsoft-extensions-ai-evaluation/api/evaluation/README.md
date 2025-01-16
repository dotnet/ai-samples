# Microsoft.Extensions.AI.Evaluation - Evaluation API Examples

This project contains a set of examples that demonstrate core concepts (such as evaluators, evaluation results,
diagnostics and result interpretation) that are included in the
[Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation) and
[Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality)
NuGet packages.

The examples in this project demonstrate how to perform evaluations without bringing in any of the reporting
functionality present in the
[Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting)
NuGet package. This can be useful in contexts where you either don't need the response caching, result storage and
report generation functionality, or where you need to store and report the results in other ways. An example of this
may be the case where you need to perform 'online' evaluation of LLM responses within your deployed production code,
and report the evaluation results via your product's existing telemetry pipeline.

**Note:** The examples included in this project invoke the LLM to perform an evaluation each time the corresponding
unit test is executed. These examples do not leverage the response caching functionality available in the
[Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting)
NuGet package, and will therefore generally run slower than the examples included in the
[Reporting API Examples](../reporting/README.md).

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
| [EvaluationExamples.cs](./EvaluationExamples.cs) | Contains setup code that is shared by all unit tests in the project. |
| [EvaluationExamples.Example01_InvokingOneEvaluator.cs](./EvaluationExamples.Example01_InvokingOneEvaluator.cs) | Demonstrates how to invoke a single evaluator. |
| [EvaluationExamples.Example02_InvokingMultipleEvaluators.cs](./EvaluationExamples.Example02_InvokingMultipleEvaluators.cs) | Demonstrates how to invoke a multiple evaluators. |
| [EvaluationExamples.Example03_InvokingEvaluatorsThatNeedAdditionalContext.cs](./EvaluationExamples.Example03_InvokingEvaluatorsThatNeedAdditionalContext.cs) | Demonstrates how to invoke evaluators that need some additional context (such as grounding context in the case of the Groundedness evaluator or baseline ground truth for the Equivalence evaluator). |
| [EvaluationExamples.Example04_InvokingCustomEvaluators.cs](./EvaluationExamples.Example04_InvokingCustomEvaluators.cs) | Demonstrates how to invoke user defined evaluators such as [`WordCountEvaluator`](./Evaluators/WordCountEvaluator.cs) and [`MeasurementSystemEvaluator`](./Evaluators/MeasurementSystemEvaluator.cs) below. |
| [MeasurementSystemEvaluator.cs](./Evaluators/MeasurementSystemEvaluator.cs) | A custom AI-based evaluator that determines which measurement system (e.g., metric, imperial, nautical etc.) is used in an LLM response. |
| [WordCountEvaluator.cs](./Evaluators/WordCountEvaluator.cs) | A custom non-AI-based evaluator that counts the number of words present in an LLM response. |
| [EvaluationExamples.Example05_AttachingDiagnosticsToMetrics.cs](./EvaluationExamples.Example05_AttachingDiagnosticsToMetrics.cs) | Demonstrates how to log diagnostics in evaluation metrics. |
| [EvaluationExamples.Example06_ChangingInterpretationOfMetrics.cs](./EvaluationExamples.Example06_ChangingInterpretationOfMetrics.cs) | Demonstrates how to define custom interpretations for evaluation metrics. |
