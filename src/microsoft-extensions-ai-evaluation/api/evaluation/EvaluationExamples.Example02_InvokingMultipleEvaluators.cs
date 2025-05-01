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
        /// Create a <see cref="CompositeEvaluator"/> that composes a <see cref="CoherenceEvaluator"/> and a
        /// <see cref="RelevanceEvaluator"/>.
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        IEvaluator relevanceEvaluator = new RelevanceEvaluator();
        IEvaluator compositeEvaluator = new CompositeEvaluator(coherenceEvaluator, relevanceEvaluator);

        /// Invoke the <see cref="CompositeEvaluator"/> to evaluate the 'coherence' and 'relevance' of the response in
        /// <see cref="s_response"/>. The evaluation is performed using the LLM endpoint configured in
        /// <see cref="s_chatConfiguration"/>.
        EvaluationResult result = await compositeEvaluator.EvaluateAsync(s_messages, s_response, s_chatConfiguration);

        using var _ = new AssertionScope();

        EvaluationRating[] expectedRatings = [EvaluationRating.Good, EvaluationRating.Exceptional];

        /// Retrieve the score for coherence from the <see cref="EvaluationResult"/>.
        NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);
        coherence.Interpretation!.Failed.Should().BeFalse(because: coherence.Interpretation.Reason);
        coherence.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: coherence.Reason);
        coherence.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        coherence.Value.Should().BeGreaterThanOrEqualTo(4, because: coherence.Reason);

        /// Retrieve the score for relevance from the <see cref="EvaluationResult"/>.
        NumericMetric relevance = result.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName);
        relevance.Interpretation!.Failed.Should().BeFalse(because: relevance.Interpretation.Reason);
        relevance.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: relevance.Reason);
        relevance.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        relevance.Value.Should().BeGreaterThanOrEqualTo(4, because: relevance.Reason);
    }
}
