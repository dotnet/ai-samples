// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Evaluators;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;

namespace Reporting;

public partial class ReportingExamples
{
    [TestMethod]
    public async Task Example06_AttachingDiagnosticsToMetrics()
    {
        await using ScenarioRun scenarioRun =
            await s_defaultReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the planet Neptune from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        using var _ = new AssertionScope();

        /// Diagnostics can be used to log additional information, warnings and / or exceptional conditions encountered
        /// during evaluation. The logged diagnostics can be useful for downstream analysis to ascertain the
        /// trustworthiness of the evaluation results. Logged diagnostics for each metric are also displayed in reports
        /// generated using the reporting APIs present in the Microsoft.Extensions.AI.Evaluation.Reporting NuGet
        /// package.
        ///
        /// Diagnostics are typically logged by <see cref="IEvaluator"/>s during evaluation. However, they can also be
        /// logged after the fact as demonstrated below.

        /// Retrieve the measurement system <see cref="StringMetric"/> from the <see cref="EvaluationResult"/> and
        /// attach some diagnostics to this <see cref="StringMetric"/>.
        StringMetric measurementSystem =
            result.Get<StringMetric>(MeasurementSystemEvaluator.MeasurementSystemMetricName);
        measurementSystem.AddDiagnostic(EvaluationDiagnostic.Informational("An informational diagnostic."));
        measurementSystem.AddDiagnostic(EvaluationDiagnostic.Warning("A warning diagnostic."));

        /// Retrieve the word count <see cref="NumericMetric"/> from the <see cref="EvaluationResult"/> and attach some
        /// diagnostics to this <see cref="NumericMetric"/>.
        NumericMetric wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);
        wordCount.AddDiagnostic(EvaluationDiagnostic.Error("An error diagnostic."));
        wordCount.AddDiagnostic(EvaluationDiagnostic.Warning("A warning diagnostic."));
        wordCount.AddDiagnostic(EvaluationDiagnostic.Informational("An informational diagnostic."));

        /// Validate that the diagnostics attached above are available on the corresponding metrics.
        measurementSystem.ContainsDiagnostics().Should().BeTrue();
        measurementSystem.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Error).Should().BeFalse();
        measurementSystem.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Warning).Should().BeTrue();
        measurementSystem.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Informational).Should().BeTrue();

        wordCount.ContainsDiagnostics().Should().BeTrue();
        wordCount.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Error).Should().BeTrue();
        wordCount.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Warning).Should().BeTrue();
        wordCount.ContainsDiagnostics(d => d.Severity is EvaluationDiagnosticSeverity.Informational).Should().BeTrue();

        /// After running all tests, inspect the generated report to understand how the diagnostics for each metric
        /// above are surfaced in the report for the current test.
        /// 
        /// Note that because we included an error diagnostic the report will mark the current evaluation as failed
        /// (and color it red).
    }
}
