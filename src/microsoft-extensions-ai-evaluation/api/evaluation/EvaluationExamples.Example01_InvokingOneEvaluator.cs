// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;

namespace Evaluation;

public partial class EvaluationExamples
{
    [TestMethod]
    public async Task Example01_InvokingOneEvaluator()
    {
        /// Invoke <see cref="CoherenceEvaluator"/> to evaluate the 'coherence' of the response in
        /// <see cref="s_response"/>. The evaluation is performed using the LLM endpoint configured in
        /// <see cref="s_chatConfiguration"/>.
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        EvaluationResult result = await coherenceEvaluator.EvaluateAsync(s_messages, s_response, s_chatConfiguration);

        using var _ = new AssertionScope();

        /// The returned <see cref="EvaluationResult"/> will contain one or more <see cref="EvaluationMetric"/>s.
        /// 
        /// Each <see cref="EvaluationMetric"/> can be one of the following types:
        /// - <see cref="NumericMetric"/>: Contains a numeric value that typically used to represent numeric scores
        ///   that fall within a well defined range (such as the coherence score below).
        /// - <see cref="StringMetric"/>: Contains a string value that is typically used to represent a single value in
        ///   an enumeration (or to represent one value out of a set of possible values). See
        ///   <see cref="Evaluators.MeasurementSystemEvaluator"/> for an example of how <see cref="StringMetric"/> can
        ///   be used.
        /// - <see cref="BooleanMetric"/>: Contains a boolean value that represents an outcome that can have one of two
        ///   possible values (such as yes v/s no, or pass v/s fail).
        ///
        /// Retrieve the score for coherence from the <see cref="EvaluationResult"/>.
        NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);

        /// Evaluators such as <see cref="CoherenceEvaluator"> above can include a default interpretation for the
        /// metrics they return. As demonstrated in <see cref="Example06_ChangingInterpretationOfMetrics"/>, the default
        /// interpretation can also be changed after the fact to suit your specific requirements if needed.
        /// 
        /// Validate the default interpretation for the returned coherence metric.
        coherence.Interpretation!.Failed.Should().NotBe(true);
        coherence.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);

        /// Evaluators such as <see cref="CoherenceEvaluator"/> above can include diagnostics on the metrics they
        /// return to indicate errors, warnings, or other exceptional conditions encountered during evaluation. As
        /// demonstrated in <see cref="Example05_AttachingDiagnosticsToMetrics"/>, diagnostics can also be attached
        /// after the fact if needed.
        /// 
        /// Validate that no diagnostics are present on the returned coherence metric.
        coherence.ContainsDiagnostics().Should().BeFalse();

        /// The coherence score returned by the evaluator is be a number between 1, and 5 with 1 representing a poor
        /// score, and 5 representing an excellent score. While it is possible to validate the returned value like
        /// below, relying on <see cref="EvaluationMetric.Interpretation"/> above is more robust since it provides a
        /// standard mechanism to determine how 'good' the metric is considered and whether it is considered 'pass' or
        /// 'fail'.
        /// 
        /// Validate that the returned coherence score is greater than 3.
        coherence.Value.Should().BeGreaterThanOrEqualTo(3);
    }
}
