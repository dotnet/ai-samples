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
    public async Task Example06_ChangingInterpretationOfMetrics()
    {
        /// Invoke <see cref="WordCountEvaluator"/> to evaluate the number of words present in the
        /// <see cref="s_response"/> (without using any AI). Note that we don't need to pass
        /// <see cref="s_chatConfiguration"/> in this case since the evaluator does not need to interact with an LLM.
        IEvaluator wordCountEvaluator = new WordCountEvaluator();
        EvaluationResult result = await wordCountEvaluator.EvaluateAsync(s_messages, s_response);

        using var _ = new AssertionScope();

        /// Retrieve the word count from the <see cref="EvaluationResult"/>.
        NumericMetric wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);

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
        wordCount.Interpretation = InterpretWordCount(wordCount);

        wordCount.Interpretation!.Failed.Should().BeFalse();
    }


    private static EvaluationMetricInterpretation? InterpretWordCount(NumericMetric metric)
    {
        if (metric is not NumericMetric wordCount ||
            metric.Name is not WordCountEvaluator.WordCountMetricName)
        {
            /// Return <see langword="null"/> to leave the existing interpretation unchanged for any metrics other than
            /// the word count metric.
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
