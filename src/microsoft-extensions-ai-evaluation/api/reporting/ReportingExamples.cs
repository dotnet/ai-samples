// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Evaluation.Evaluators;
using Evaluation.Setup;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage.Disk;

namespace Reporting;

[TestClass]
public partial class ReportingExamples
{
    /// Note: The value of the below property is populated by MSTest.
    public TestContext? TestContext { get; set; }

    /// The execution name below is used to group evaluation results that are part of the same evaluation run (or test
    /// run) together when the evaluation results are stored. If an execution name is not provided when creating a
    /// <see cref="ReportingConfiguration"/>, all evaluation runs will use the same default execution name "Default".
    /// In this case, results from one run will be overwritten by the next and we would lose the ability to compare
    /// results across different runs.
    /// 
    /// For the current sample where all the evaluation tests (and <see cref="ReportingConfiguration"/>s) that need the
    /// evaluation name live in the same assembly, we use a timestamp (that is computed once and stored in a static
    /// variable) as the execution name. To ensure that results are grouped correctly, we make sure to use the same
    /// execution name in all <see cref="ReportingConfiguration"/>s used across the tests in the current project
    /// (including <see cref="s_defaultReportingConfiguration"/>,
    /// <see cref="s_reportingConfigurationWithCachingDisabled"/>,
    /// <see cref="s_reportingConfigurationWithEquivalenceAndGroundedness"/> and
    /// <see cref="s_sqliteReportingConfiguration"/>).
    /// 
    /// In a more real-world scenario, you may want to share the same execution name across evaluation tests that live
    /// in multiple different assemblies and that end up being executed in different test processes. In such cases, you
    /// would need to use some other strategy to share the same execution name across these evaluation tests. For
    /// example, you could use a script to update an environment variable with an appropriate execution name (such as
    /// the current build number assigned by your CI/CD system) before running the tests. Or if your build system
    /// produces monotonically increasing assembly file versions, you could read the
    /// <see cref="AssemblyFileVersionAttribute"/> from within the test code and use that as the execution name to
    /// compare results across different product versions.
    /// 
    /// Note that because we use a timestamp as the execution name below, generated reports (such as the ones generated
    /// in <see cref="Example10_GeneratingReportProgrammatically"/> and
    /// <see cref="Example11_GeneratingReportProgrammaticallyFromCustomStorage"/>) will include evaluation results from
    /// all tests in the current project only if these tests are all executed together as part of the same test run. If
    /// the tests are executed one at a time instead (using the IDE's test runner for example), the generated report
    /// will only include the results from the single test that was executed last.

    private static string? s_executionName;
    public static string ExecutionName
    {
        get
        {
            if (s_executionName is null)
            {
                s_executionName = $"{DateTime.Now:yyyyMMddTHHmmss}";
            }

            return s_executionName;
        }
    }

    /// For simplicity, we use the fully qualified name of the current test method as the name of the corresponding
    /// scenario being evaluated (i.e. as the <see cref="ScenarioRun.ScenarioName"/>) in each unit test. However, the
    /// scenario name can be set to any string of your choice when you call
    /// <see cref="ReportingConfiguration.CreateScenarioRunAsync(string, string, IEnumerable{string}?, CancellationToken)"/>.
    /// 
    /// Here are a couple of considerations worth keeping in mind when choosing a scenario name:
    /// * When using disk-based storage, the scenario name is used as the name of the folder under which the
    ///   corresponding evaluation results are stored. So it would be a good idea to keep the name reasonably short and
    ///   avoid any characters that are not allowed in file and directory names.
    /// * By default, the generated evaluation report will split scenario names on '.' so that the results can be
    ///   displayed in a hierarchical view with appropriate grouping, nesting and aggregation. This is especially
    ///   useful in cases like below where the scenario name is set to the fully qualified name of the corresponding
    ///   test method, since this will allow the results to be grouped by namespaces and class names in the hierarchy.
    ///   However, you can also take advantage of this feature by including '.'s in your own custom scenario names to
    ///   create a reporting hierarchy that works best for the scenarios that you are evaluating.

    private string ScenarioName => $"{TestContext!.FullyQualifiedTestClassName}.{TestContext.TestName}";

    /// A <see cref="ReportingConfiguration"/> identifies:
    /// - the set of evaluators that should be invoked for each <see cref="ScenarioRun"/> that is created by calling
    ///   <see cref="ReportingConfiguration.CreateScenarioRunAsync(string, string, IEnumerable{string}?, CancellationToken)"/>
    /// - the LLM endpoint that these evaluators should use (see <see cref="ReportingConfiguration.ChatConfiguration"/>)
    /// - how / where the results for these <see cref="ScenarioRun"/>s should be stored
    /// - whether / how LLM responses related to these <see cref="ScenarioRun"/>s should be cached
    /// - the execution name that should be used when reporting results for these <see cref="ScenarioRun"/>s
    /// 
    /// The following (disk-based) <see cref="ReportingConfiguration"/> is used by most sample tests in the current
    /// project. A few tests also use other <see cref="ReportingConfiguration"/>s (such as
    /// <see cref="s_reportingConfigurationWithCachingDisabled"/>,
    /// <see cref="s_reportingConfigurationWithEquivalenceAndGroundedness"/> and
    /// <see cref="s_sqliteReportingConfiguration"/> all defined within the corresponding test files) to demonstrate
    /// other concepts.

    private static readonly ReportingConfiguration s_defaultReportingConfiguration =
        DiskBasedReportingConfiguration.Create(
            storageRootPath: EnvironmentVariables.StorageRootPath,
            evaluators: GetEvaluators(),
            chatConfiguration: TestSetup.GetChatConfiguration(),
            enableResponseCaching: true,
            executionName: ExecutionName);

    /// Most sample tests in the current project run the following 3 evaluators to evaluate LLM responses
    /// for a variety of astronomy questions related to distances between planets. Some tests (such as
    /// <see cref="Example05_InvokingEvaluatorsThatNeedAdditionalContext"/>) run other evaluators to demonstrate
    /// other concepts.

    private static IEnumerable<IEvaluator> GetEvaluators()
    {
        var rtcOptions = new RelevanceTruthAndCompletenessEvaluator.Options(includeReasoning: true);
        IEvaluator rtcEvaluator = new RelevanceTruthAndCompletenessEvaluator(rtcOptions);
        IEvaluator measurementSystemEvaluator = new MeasurementSystemEvaluator();
        IEvaluator wordCountEvaluator = new WordCountEvaluator();

        return [rtcEvaluator, measurementSystemEvaluator, wordCountEvaluator];
    }

    /// All sample tests in the current project evaluate the LLM's response to a different
    /// <paramref name="astronomyQuestion"/>. Since all <see cref="ReportingConfiguration"/>s used across the tests
    /// have response caching turned on, and since the supplied <paramref name="chatClient"/> is always fetched from
    /// the <see cref="ScenarioRun"/> created using this <see cref="ReportingConfiguration"/> (i.e., via
    /// <see cref="ScenarioRun.ChatConfiguration"/>), the LLM responses for each test are cached and reused until the
    /// corresponding cache entry expires (in 14 days by default), or until any request parameter (such as the question
    /// being asked, or the LLM endpoint being invoked) is changed.

    private static async Task<(IList<ChatMessage> Messages, ChatMessage ModelResponse)> GetAstronomyConversationAsync(
        IChatClient chatClient,
        string astronomyQuestion)
    {
        const string SystemPrompt =
            """
            You are an AI assistant that can answer questions related to astronomy.
            Keep your responses concise staying under 100 words as much as possible.
            Use the imperial measurement system for all measurements in your response.
            """;

        IList<ChatMessage> messages =
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                new ChatMessage(ChatRole.User, astronomyQuestion)
            ];

        var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text
            };

        ChatCompletion completion = await chatClient.CompleteAsync(messages, chatOptions);
        return (messages, ModelResponse: completion.Message);
    }

    /// <summary>
    /// Runs some basic validation on the supplied <see cref="EvaluationResult"/>.
    /// </summary>
    private static void Validate(EvaluationResult result)
    {
        using var _ = new AssertionScope();

        /// Note that since we said 'includeReasoning: true' when creating the
        /// <see cref="RelevanceTruthAndCompletenessEvaluator"/> inside <see cref="GetEvaluators"/> above, the
        /// 'relevance', 'truth' and 'completeness' metrics will will each include a single informational diagnostic
        /// explaining the reasoning for the score. This diagnostic is included in the generated report and can be
        /// viewed by hovering over the corresponding metric's card in the report.

        /// Retrieve the score for relevance from the <see cref="EvaluationResult"/>.
        NumericMetric relevance =
            result.Get<NumericMetric>(RelevanceTruthAndCompletenessEvaluator.RelevanceMetricName);
        relevance.Interpretation!.Failed.Should().NotBe(true);
        relevance.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        relevance.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for truth from the <see cref="EvaluationResult"/>.
        NumericMetric truth = result.Get<NumericMetric>(RelevanceTruthAndCompletenessEvaluator.TruthMetricName);
        truth.Interpretation!.Failed.Should().NotBe(true);
        truth.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        truth.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the score for completeness from the <see cref="EvaluationResult"/>.
        NumericMetric completeness =
            result.Get<NumericMetric>(RelevanceTruthAndCompletenessEvaluator.CompletenessMetricName);
        completeness.Interpretation!.Failed.Should().NotBe(true);
        completeness.Interpretation.Rating.Should().BeOneOf(EvaluationRating.Good, EvaluationRating.Exceptional);
        completeness.Value.Should().BeGreaterThanOrEqualTo(3);

        /// Retrieve the measurement system from the <see cref="EvaluationResult"/>.
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
