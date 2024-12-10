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
    public async Task Example07_ChangingInterpretationOfMetrics()
    {
        await using ScenarioRun scenarioRun =
            await s_defaultReportingConfiguration.CreateScenarioRunAsync(this.ScenarioName);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the planet Uranus from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        using var _ = new AssertionScope();

        /// <see cref="WordCountEvaluator"/> provides a default <see cref="EvaluationMetric.Interpretation"/> for the
        /// word count metric it returns. However, you can re-interpret any metric based on your own criteria. In the
        /// below example, we re-interpret the word count metric such that any response that is either empty or that
        /// contains more than 200 words is considered unacceptable, any response that contains less than 20 words is
        /// considered poor, and any response that contains between 20 to 100 words is considered good.
        /// 
        /// Interpretations are mainly used for reporting purposes although they can also be useful for downstream
        /// analysis of metrics. They provide a consistent way to categorize different kinds of metrics. The data
        /// present within interpretations also determine how the metrics are displayed (color, pass v/s fail, etc.)
        /// in reports generated using the reporting APIs present in the the
        /// Microsoft.Extensions.AI.Evaluation.Reporting NuGet package.
        result.Interpret(
            metric => metric is NumericMetric { Name: WordCountEvaluator.WordCountMetricName } numericMetric
                ? InterpretWordCount(numericMetric)
                : null); /// Return null to leave the interpretation for metrics other than word count unchanged.

        /// Retrieve the measurement system metric from the <see cref="EvaluationResult"/>.
        StringMetric measurementSystem =
            result.Get<StringMetric>(MeasurementSystemEvaluator.MeasurementSystemMetricName);

        /// Explicitly set the interpretation for the measurement system metric to null to remove the original
        /// interpretation supplied by the <see cref="MeasurementSystemEvaluator"/>.
        measurementSystem.Interpretation = null;

        /// Retrieve the word count from the <see cref="EvaluationResult"/>.
        NumericMetric wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);

        wordCount.Interpretation!.Failed.Should().BeFalse();

        /// After running all tests, inspect the generated report to understand how the interpretation changes applied
        /// to the above metrics are surfaced in the report for the current test.
        /// 
        /// Notice how the card for the measurement system metric (which was updated to have no interpretation above)
        /// is displayed as greyed out in the report.
    }

    private static EvaluationMetricInterpretation? InterpretWordCount(NumericMetric metric)
    {
        if (metric is not NumericMetric wordCount ||
            metric.Name is not WordCountEvaluator.WordCountMetricName)
        {
            return null;
        }

        if (wordCount.Value is null)
        {
            return new EvaluationMetricInterpretation(
                EvaluationRating.Unacceptable,
                failed: true,
                reason: "The response was empty");
        }

        return wordCount.Value switch
        {
            0 =>
                new EvaluationMetricInterpretation(
                    EvaluationRating.Unacceptable,
                    failed: true,
                    reason: "The response was empty"),
            <= 20 =>
                new EvaluationMetricInterpretation(
                    EvaluationRating.Poor,
                    failed: true,
                    reason: "The response was too short"),
            <= 100 =>
                new EvaluationMetricInterpretation(
                    EvaluationRating.Good,
                    reason: "The response was of an acceptable length"),
            <= 200 =>
                new EvaluationMetricInterpretation(
                    EvaluationRating.Poor,
                    failed: true,
                    reason: "The response was long"),
            _ =>
                new EvaluationMetricInterpretation(
                    EvaluationRating.Unacceptable,
                    failed: true,
                    reason: "The response was too long")
        };
    }
}
