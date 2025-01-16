// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Setup;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage.Disk;

namespace Reporting;

public partial class ReportingExamples
{
    private static readonly ReportingConfiguration s_reportingConfigurationWithEquivalenceAndGroundedness =
        DiskBasedReportingConfiguration.Create(
            storageRootPath: EnvironmentVariables.StorageRootPath,
            evaluators: [new EquivalenceEvaluator(), new GroundednessEvaluator()],
            chatConfiguration: TestSetup.GetChatConfiguration(),
            enableResponseCaching: true,
            executionName: ExecutionName);

    [TestMethod]
    public async Task Example05_InvokingEvaluatorsThatNeedAdditionalContext()
    {
        /// This example is similar to the one in
        /// <see cref="Example01_SamplingAndEvaluatingSingleResponse"/> with the main difference being that this
        /// example uses an alternate <see cref="ReportingConfiguration"/> with a different set of evaluators
        /// (<see cref="s_reportingConfigurationWithEquivalenceAndGroundedness"/> above).

        await using ScenarioRun scenarioRun =
            await s_reportingConfigurationWithEquivalenceAndGroundedness.CreateScenarioRunAsync(this.ScenarioName);

        var (messages, modelResponse) = await GetAstronomyConversationAsync(
            chatClient: scenarioRun.ChatConfiguration!.ChatClient,
            astronomyQuestion: "How far is the planet Venus from the Earth at its closest and furthest points?");

        /// Create an instance of <see cref="EquivalenceEvaluator.Context"/> that contains a baseline response against
        /// which the <see cref="EquivalenceEvaluator"/> should compare <see cref="modelResponse"/> in order to
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
        /// 'groundedness' score which indicates how well <see cref="modelResponse"/> is grounded in the supplied
        /// grounding context.
        GroundednessEvaluator.Context groundingContextForGroundednessEvaluator =
            new GroundednessEvaluator.Context(
                """
                Distance between Venus and Earth at inferior conjunction: About 23.6 million miles.
                Distance between Venus and Earth at superior conjunction: About 162 million miles.
                """);

        /// Run the evaluators against the above response.
        EvaluationResult result =
            await scenarioRun.EvaluateAsync(
                messages,
                modelResponse,
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
