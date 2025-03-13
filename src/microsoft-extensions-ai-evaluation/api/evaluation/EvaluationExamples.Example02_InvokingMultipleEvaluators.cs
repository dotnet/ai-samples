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
        /// <see cref="FluencyEvaluator"/>.
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        IEvaluator fluencyEvaluator = new FluencyEvaluator();
        IEvaluator compositeEvaluator = new CompositeEvaluator(coherenceEvaluator, fluencyEvaluator);

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
    }
}
