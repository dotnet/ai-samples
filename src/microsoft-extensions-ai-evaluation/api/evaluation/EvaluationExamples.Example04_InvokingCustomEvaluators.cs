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
    public async Task Example04_InvokingCustomEvaluators()
    {
        /// Create a <see cref="CompositeEvaluator"/> that composes a <see cref="MeasurementSystemEvaluator"/> and a
        /// <see cref="WordCountEvaluator"/>.
        /// - <see cref="MeasurementSystemEvaluator"/> is a custom evaluator defined within the current project that
        ///   uses AI to identify the measurement system used in <see cref="s_response"/>.
        /// - <see cref="WordCountEvaluator"/> is another custom evaluator defined within the current project that
        ///   simply counts the number of words present in <see cref="s_response"/> (without using any AI).
        IEvaluator measurementSystemEvaluator = new MeasurementSystemEvaluator();
        IEvaluator wordCountEvaluator = new WordCountEvaluator();
        IEvaluator compositeEvaluator = new CompositeEvaluator(measurementSystemEvaluator, wordCountEvaluator);

        /// Invoke the <see cref="CompositeEvaluator"/> to identify the measurement system and the number of words.
        /// The measurement system identification is performed using the LLM endpoint configured in
        /// <see cref="s_chatConfiguration"/>.
        EvaluationResult result = await compositeEvaluator.EvaluateAsync(s_messages, s_response, s_chatConfiguration);

        using var _ = new AssertionScope();

        /// Retrieve the detected measurement system from the <see cref="EvaluationResult"/>.
        StringMetric measurementSystem =
            result.Get<StringMetric>(MeasurementSystemEvaluator.MeasurementSystemMetricName);
        measurementSystem.Interpretation!.Failed.Should().NotBe(true);
        measurementSystem.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        measurementSystem.ContainsDiagnostics().Should().BeFalse();
        measurementSystem.Value.Should().Be(nameof(MeasurementSystemEvaluator.MeasurementSystem.Imperial));

        /// Retrieve the word count from the <see cref="EvaluationResult"/>.
        NumericMetric wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);
        wordCount.Interpretation!.Failed.Should().NotBe(true);
        wordCount.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        wordCount.ContainsDiagnostics().Should().BeFalse();
        wordCount.Value.Should().BeLessThanOrEqualTo(100);
    }
}
