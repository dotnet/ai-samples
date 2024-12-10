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
    public async Task Example02_InvokingMultipleEvaluators()
    {
        /// Create a <see cref="CompositeEvaluator"/> that composes a <see cref="CoherenceEvaluator"/>, a
        /// <see cref="FluencyEvaluator"/> and a <see cref="RelevanceTruthAndCompletenessEvaluator"/>.
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        IEvaluator fluencyEvaluator = new FluencyEvaluator();
        IEvaluator rtcEvaluator = new RelevanceTruthAndCompletenessEvaluator();
        IEvaluator compositeEvaluator = new CompositeEvaluator(coherenceEvaluator, fluencyEvaluator, rtcEvaluator);

        /// Invoke the <see cref="CompositeEvaluator"/> to evaluate the 'coherence', 'fluency', 'relevance', 'truth'
        /// and 'completeness' of the response in <see cref="s_response"/>. The evaluation is performed using the LLM
        /// endpoint configured in <see cref="s_chatConfiguration"/>.
        EvaluationResult result = await compositeEvaluator.EvaluateAsync(s_messages, s_response, s_chatConfiguration);

        using var _ = new AssertionScope();

        /// Retrieve the score for coherence from the <see cref="EvaluationResult"/>.
        NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);
        coherence.Interpretation!.Failed.Should().NotBe(true);
        coherence.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        coherence.ContainsDiagnostics().Should().BeFalse();
        coherence.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for fluency from the <see cref="EvaluationResult"/>.
        NumericMetric fluency = result.Get<NumericMetric>(FluencyEvaluator.FluencyMetricName);
        fluency.Interpretation!.Failed.Should().NotBe(true);
        fluency.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        fluency.ContainsDiagnostics().Should().BeFalse();
        fluency.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for relevance from the <see cref="EvaluationResult"/>. 
        NumericMetric relevance =
            result.Get<NumericMetric>(RelevanceTruthAndCompletenessEvaluator.RelevanceMetricName);
        relevance.Interpretation!.Failed.Should().NotBe(true);
        relevance.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        relevance.ContainsDiagnostics().Should().BeFalse();
        relevance.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for truth from the <see cref="EvaluationResult"/>. 
        NumericMetric truth = result.Get<NumericMetric>(RelevanceTruthAndCompletenessEvaluator.TruthMetricName);
        truth.Interpretation!.Failed.Should().NotBe(true);
        truth.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        truth.ContainsDiagnostics().Should().BeFalse();
        truth.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for completeness from the <see cref="EvaluationResult"/>. 
        NumericMetric completeness =
            result.Get<NumericMetric>(RelevanceTruthAndCompletenessEvaluator.CompletenessMetricName);
        completeness.Interpretation!.Failed.Should().NotBe(true);
        completeness.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        completeness.ContainsDiagnostics().Should().BeFalse();
        completeness.Value.Should().BeGreaterThanOrEqualTo(3);
    }
}
