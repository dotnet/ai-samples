// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Evaluation.Setup;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.NLP;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

namespace Reporting;

public partial class ReportingExamples
{
    private static readonly ReportingConfiguration s_reportingConfigurationWithNLPEvaluators =
       DiskBasedReportingConfiguration.Create(
           storageRootPath: EnvironmentVariables.StorageRootPath,
           evaluators: [new BLEUEvaluator(), new GLEUEvaluator(), new F1Evaluator()],
           chatConfiguration: s_chatConfiguration,
           enableResponseCaching: true,
           executionName: ExecutionName,
           tags: GetGlobalTags());

    [TestMethod]
    public async Task Example12_RunningNLPEvaluators()
    {
        /// This example demonstrates how to run NLP (Natural Language Processing) evaluators (such as
        /// <see cref="BLEUEvaluator"/>, <see cref="GLEUEvaluator"/>, and <see cref="F1Evaluator"/>) that measure text
        /// similarity between a model's output and supplied reference text. These evaluators can be useful to assess
        /// the quality of AI-generated text in various scenarios such as machine translation, summarization and
        /// content generation.
        /// 
        /// Note that unlike the evaluators demonstrated in other examples, the NLP evaluators below do not require an
        /// LLM to perform the evaluation. Instead, they use traditional NLP techniques (text tokenization, n-gram
        /// analysis, etc.) to compute text similarity scores.

        await using ScenarioRun scenarioRun =
            await s_reportingConfigurationWithNLPEvaluators.CreateScenarioRunAsync(
                this.ScenarioName,
                additionalTags: ["Paris"]);

        /// Get a conversation that simulates an AI-assisted text generation task for NLP evaluation.
        (IList<ChatMessage> messages, ChatResponse response, IList<string> referenceResponses) =
            await GetConversationForNLPEvaluationAsync(scenarioRun.ChatConfiguration!.ChatClient);

        /// The NLP evaluators require one or more reference responses to compare against the model's output.
        var referenceResponsesForBLEU = new BLEUEvaluatorContext(referenceResponses);
        var referenceResponsesForGLEU = new GLEUEvaluatorContext(referenceResponses);
        var groundTruthForF1 = new F1EvaluatorContext(groundTruth: referenceResponses.First());

        /// Run the NLP evaluators against the response.
        EvaluationResult result =
            await scenarioRun.EvaluateAsync(
                messages,
                response,
                additionalContext: [referenceResponsesForBLEU, referenceResponsesForGLEU, groundTruthForF1]);

        /// Run some basic validation on the evaluation result.
        ValidateNLPMetrics(result);
    }

    private static async Task<(IList<ChatMessage> Messages, ChatResponse ModelResponse, IList<string> ReferenceResponses)>
        GetConversationForNLPEvaluationAsync(IChatClient chatClient)
    {
        const string SystemPrompt =
            """
            You are a knowledgeable assistant.
            Provide accurate and helpful information.
            Keep your answers short (50 words or less).
            """;

        const string UserQuery = "What is the capital of France and what is it known for?";

        const string ReferenceResponse1 =
            """
            Paris is the capital of France. It is renowned for the Eiffel Tower, Louvre Museum, Notre-Dame Cathedral,
            fine dining, fashion industry, artistic heritage, and rich cultural traditions.
            """;

        const string ReferenceResponse2 =
            """
            Paris, the capital of France, is famous for its iconic landmarks like the Eiffel Tower, Louvre Museum and
            Notre-Dame Cathedral, its vibrant culture, art, fashion, and cuisine. It is often referred to as the "City
            of Light".
            """;

        const string ReferenceResponse3 =
            """
            The capital of France is Paris. Paris is known for its rich history, art, and culture. It is home to iconic
            landmarks such as the Eiffel Tower, Louvre Museum, and Notre-Dame Cathedral. The city is also famous for
            its cuisine, fashion, and vibrant atmosphere.
            """;

        List<ChatMessage> messages =
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                new ChatMessage(ChatRole.User, UserQuery)
            ];

        var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text
            };

        ChatResponse response = await chatClient.GetResponseAsync(messages, chatOptions);
        return (messages, response, [ReferenceResponse1, ReferenceResponse2, ReferenceResponse3]);
    }

    private static void ValidateNLPMetrics(EvaluationResult result)
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

        /// Retrieve the score for BLEU from the <see cref="EvaluationResult"/>.
        NumericMetric bleu = result.Get<NumericMetric>(BLEUEvaluator.BLEUMetricName);
        bleu.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        bleu.Value.Should().BeGreaterThan(0, because: bleu.Reason).And.BeLessThan(1, because: bleu.Reason);

        /// Retrieve the score for GLEU from the <see cref="EvaluationResult"/>.
        NumericMetric gleu = result.Get<NumericMetric>(GLEUEvaluator.GLEUMetricName);
        gleu.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        gleu.Value.Should().BeGreaterThan(0, because: gleu.Reason).And.BeLessThan(1, because: gleu.Reason);

        /// Retrieve the score for F1 from the <see cref="EvaluationResult"/>.
        NumericMetric f1 = result.Get<NumericMetric>(F1Evaluator.F1MetricName);
        f1.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        f1.Value.Should().BeGreaterThan(0, because: f1.Reason).And.BeLessThan(1, because: f1.Reason);
    }
}
