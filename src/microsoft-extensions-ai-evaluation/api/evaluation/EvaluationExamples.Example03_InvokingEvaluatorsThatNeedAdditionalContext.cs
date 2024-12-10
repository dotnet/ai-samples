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
    public async Task Example03_InvokingEvaluatorsThatNeedAdditionalContext()
    {
        /// Create a <see cref="CompositeEvaluator"/> that composes an <see cref="EquivalenceEvaluator"/> and a
        /// <see cref="GroundednessEvaluator"/>.
        IEvaluator equivalenceEvaluator = new EquivalenceEvaluator();
        IEvaluator groundednessEvaluator = new GroundednessEvaluator();
        IEvaluator compositeEvaluator = new CompositeEvaluator(equivalenceEvaluator, groundednessEvaluator);

        /// Create an instance of <see cref="EquivalenceEvaluator.Context"/> that contains a baseline response against
        /// which the <see cref="EquivalenceEvaluator"/> should compare <see cref="s_response"/> in order to
        /// generate an 'equivalence' score.
        EquivalenceEvaluator.Context baselineResponseForEquivalenceEvaluator =
            new EquivalenceEvaluator.Context(
                """
                The distance between Earth and Venus varies significantly due to the elliptical orbits of both planets
                around the Sun. At their closest approach, known as inferior conjunction, Venus can be about 23.6
                million miles away from Earth. At their furthest point, when Venus is on the opposite side of the Sun
                from Earth, known as superior conjunction, the distance can be about 162 million miles. These distances
                can vary slightly due to the specific orbital positions of the planets at any given time.
                """);

        /// Create an instance of <see cref="GroundednessEvaluator.Context"/> that contains grounding context that
        /// the <see cref="GroundednessEvaluator"/> should use. The <see cref="GroundednessEvaluator"/> will produce a
        /// 'groundedness' score which indicates how well <see cref="s_response"/> is grounded in the supplied
        /// grounding context.
        GroundednessEvaluator.Context groundingContextForGroundednessEvaluator =
            new GroundednessEvaluator.Context(
                """
                Distance between Venus and Earth at inferior conjunction: About 23.6 million miles.
                Distance between Venus and Earth at superior conjunction: About 162 million miles.
                """);

        /// Invoke the <see cref="CompositeEvaluator"/> to evaluate the equivalence and groundedness of the response in
        /// <see cref="s_response"/> in relation to the baseline response and grounding context provided above. The
        /// evaluation is performed using the LLM endpoint configured in <see cref="s_chatConfiguration"/>.
        EvaluationResult result =
            await compositeEvaluator.EvaluateAsync(
                s_messages,
                s_response,
                s_chatConfiguration,
                [baselineResponseForEquivalenceEvaluator, groundingContextForGroundednessEvaluator]);

        using var _ = new AssertionScope();

        /// Retrieve the score for equivalence from the <see cref="EvaluationResult"/>.
        NumericMetric equivalence = result.Get<NumericMetric>(EquivalenceEvaluator.EquivalenceMetricName);
        equivalence.Interpretation!.Failed.Should().NotBe(true);
        equivalence.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        equivalence.ContainsDiagnostics().Should().BeFalse();
        equivalence.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for groundedness from the <see cref="EvaluationResult"/>.
        NumericMetric groundedness = result.Get<NumericMetric>(GroundednessEvaluator.GroundednessMetricName);
        groundedness.Interpretation!.Failed.Should().NotBe(true);
        groundedness.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        groundedness.ContainsDiagnostics().Should().BeFalse();
        groundedness.Value.Should().BeGreaterThanOrEqualTo(3);
    }
}
