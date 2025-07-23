// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Evaluation.Evaluators;
using Evaluation.Setup;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

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
    /// execution name live in the same assembly, we use a timestamp (that is computed once and stored in a static
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
    /// in <see cref="Example17_GeneratingReportProgrammatically"/>,
    /// <see cref="Example18_GeneratingReportProgrammaticallyFromAzureStorage"/> and
    /// <see cref="Example19_GeneratingReportProgrammaticallyFromCustomStorage"/>) will include evaluation results from
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

    /// The below <see cref="ChatConfiguration"/> identifies the LLM endpoint that should be used for all evaluations
    /// performed in the current sample project.
    private static readonly ChatConfiguration s_chatConfiguration = TestSetup.GetChatConfiguration();

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
    /// <see cref="s_reportingConfigurationWithEquivalenceAndGroundedness"/>,
    /// <see cref="s_sqliteReportingConfiguration"/>, <see cref="s_safetyReportingConfiguration"/> etc., all defined
    /// within the corresponding test files) to demonstrate other concepts.

    private static readonly ReportingConfiguration s_defaultReportingConfiguration =
        DiskBasedReportingConfiguration.Create(
            storageRootPath: EnvironmentVariables.StorageRootPath,
            evaluators: GetEvaluators(),
            chatConfiguration: s_chatConfiguration,
            enableResponseCaching: true,
            executionName: ExecutionName,
            tags: GetTags());

    /// Most sample tests in the current project run the following evaluators to evaluate LLM responses for a variety
    /// of astronomy questions related to distances between planets. Some tests (such as
    /// <see cref="Example05_InvokingEvaluatorsThatNeedAdditionalContext"/>,
    /// <see cref="Example08_RunningSafetyEvaluators"/> etc.) run other evaluators to demonstrate other concepts.

    private static IEnumerable<IEvaluator> GetEvaluators()
    {
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        yield return coherenceEvaluator;

        IEvaluator relevanceEvaluator = new RelevanceEvaluator();
        yield return relevanceEvaluator;

        IEvaluator measurementSystemEvaluator = new MeasurementSystemEvaluator();
        yield return measurementSystemEvaluator;

        IEvaluator wordCountEvaluator = new WordCountEvaluator();
        yield return wordCountEvaluator;
    }

    /// We create tag strings for provider name, model name etc. below that we then pass to the
    /// <see cref="ReportingConfiguration"/> constructor above. Tags are can be useful when it comes to viewing high
    /// level information about the evaluation run in the generated report. They can also be useful for easily
    /// filtering the results in the generated report.

    private static IEnumerable<string> GetTags(string storageKind = "Disk")
    {
        foreach (string tag in GetGlobalTags(storageKind))
        {
            yield return tag;
        }

        ChatClientMetadata? metadata = s_chatConfiguration.ChatClient.GetService<ChatClientMetadata>();

        yield return $"Provider: {metadata?.ProviderName ?? "Unknown"}";
        yield return $"Model: {metadata?.DefaultModelId ?? "Unknown"}";
    }

    private static IEnumerable<string> GetGlobalTags(string storageKind = "Disk")
    {
        yield return $"Execution: {ExecutionName}";
        yield return $"Storage: {storageKind}";
    }

    /// All sample tests in the current project evaluate the LLM's response to a different
    /// <paramref name="astronomyQuestion"/>. Since all <see cref="ReportingConfiguration"/>s used across the tests
    /// have response caching turned on, and since the supplied <paramref name="chatClient"/> is always fetched from
    /// the <see cref="ScenarioRun"/> created using this <see cref="ReportingConfiguration"/> (i.e., via
    /// <see cref="ScenarioRun.ChatConfiguration"/>), the LLM responses for each test are cached and reused until the
    /// corresponding cache entry expires (in 14 days by default), or until any request parameter (such as the question
    /// being asked, or the LLM endpoint being invoked) is changed.

    private static async Task<(IList<ChatMessage> Messages, ChatResponse ModelResponse)> GetAstronomyConversationAsync(
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

        ChatResponse response = await chatClient.GetResponseAsync(messages, chatOptions);
        return (messages, response);
    }

    private static void Validate(EvaluationResult result)
    {
        /// Note: The below validation is mainly for demonstration purposes. In real-world evaluations, you may not
        /// want to validate individual results since the LLM responses and evaluation scores can jump around a bit
        /// over time as your product (and the models used) evolve. You may not want individual evaluation tests to
        /// 'fail' and block builds in your CI/CD pipelines when this happens. Instead, in such cases, it may be better
        /// to rely on the generated report and track the overall trends for evaluation scores across different
        /// scenarios over time (and only fail individual builds in your CI/CD pipelines when there is a significant
        /// drop in evaluation scores across multiple different tests). That said, there is some nuance here and the
        /// choice of whether to validate individual results or not can vary depending on the specific use case.

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

        /// Retrieve the measurement system from the <see cref="EvaluationResult"/>.
        StringMetric measurementSystem =
            result.Get<StringMetric>(MeasurementSystemEvaluator.MeasurementSystemMetricName);
        measurementSystem.Interpretation!.Failed.Should().BeFalse(because: measurementSystem.Interpretation.Reason);
        measurementSystem.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: measurementSystem.Reason);
        measurementSystem.ContainsDiagnostics(
            d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        measurementSystem.Value.Should().Be(nameof(MeasurementSystemEvaluator.MeasurementSystem.Imperial));

        /// Retrieve the word count from the <see cref="EvaluationResult"/>.
        NumericMetric wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);
        wordCount.Interpretation!.Failed.Should().BeFalse(because: wordCount.Interpretation.Reason);
        wordCount.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: wordCount.Reason);
        wordCount.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        wordCount.Value.Should().BeLessThanOrEqualTo(100);
    }
}
