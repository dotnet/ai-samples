// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Evaluators;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.AI.Evaluation;

namespace Evaluation;

public partial class EvaluationExamples
{
    [TestMethod]
    public async Task Example05_AttachingDiagnosticsToMetrics()
    {
        /// Invoke <see cref="WordCountEvaluator"/> to evaluate the number of words present in the
        /// <see cref="s_response"/> (without using any AI). Note that we don't need to pass
        /// <see cref="s_chatConfiguration"/> in this case since the evaluator does not need to interact with an LLM.
        IEvaluator wordCountEvaluator = new WordCountEvaluator();
        EvaluationResult result = await wordCountEvaluator.EvaluateAsync(s_messages, s_response);

        using var _ = new AssertionScope();

        /// Retrieve the word count from the <see cref="EvaluationResult"/>.
        NumericMetric wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);

        /// Diagnostics can be used to log additional information, warnings and / or exceptional conditions encountered
        /// during evaluation. The logged diagnostics can be useful for downstream analysis to ascertain the
        /// trustworthiness of the evaluation results. Logged diagnostics for each metric are also displayed in reports
        /// generated using the reporting APIs present in the Microsoft.Extensions.AI.Evaluation.Reporting NuGet
        /// package.
        ///
        /// Diagnostics are typically logged by <see cref="IEvaluator"/>s during evaluation. However, they can also be
        /// logged after the fact as demonstrated below.
        wordCount.AddDiagnostic(EvaluationDiagnostic.Error("An error diagnostic."));
        wordCount.AddDiagnostic(EvaluationDiagnostic.Warning("A warning diagnostic."));
        wordCount.AddDiagnostic(EvaluationDiagnostic.Informational("An informational diagnostic."));

        wordCount.ContainsDiagnostics().Should().BeTrue();
        wordCount.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Error).Should().BeTrue();
        wordCount.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Warning).Should().BeTrue();
        wordCount.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Informational).Should().BeTrue();
    }
}
