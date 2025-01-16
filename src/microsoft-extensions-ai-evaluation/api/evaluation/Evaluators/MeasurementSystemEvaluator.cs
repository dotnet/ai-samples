// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;

namespace Evaluation.Evaluators;

/// <summary>
/// An evaluator that uses AI to determine the measurement system (if any) that is used in the response that is being
/// evaluated.
/// </summary>
/// <remarks>
/// The name of the detected measurement system is returned via a <see cref="StringMetric"/> as part of the returned
/// <see cref="EvaluationResult"/>.
/// </remarks>
public class MeasurementSystemEvaluator : IEvaluator
{
    public enum MeasurementSystem
    {
        NotApplicable,
        Unknown,
        Metric,
        Imperial,
        USCustomary,
        Nautical,
        Astronomical,
        Mixed
    }

    public const string MeasurementSystemMetricName = "Measurement System";

    /// <inheritdoc/>
    public IReadOnlyCollection<string> EvaluationMetricNames => [MeasurementSystemMetricName];

    #region Prompts
    private const string EvaluationSystemPrompt =
        """
        You are an AI assistant. You will be given the answer to a question that a user has asked. Your task is
        to inspect this answer and figure out which measurement system is being used in the answer.
            
        The following measurement systems can be used in the answer: Metric System, Imperial System, US Customary Units, Nautical System or Astronomical Units.
            
        Here is a table that contains some examples of the different units of measurement that are used in each measurement system.

        | **Attribute** | **Metric System**          | **Imperial System**                        | **US Customary Units**                     | **Nautical System**             | **Astronomical Units**                     |
        |---------------|----------------------------|--------------------------------------------|--------------------------------------------|---------------------------------|--------------------------------------------|
        | Length        | meter (m), kilometer (km)  | inch (in), foot (ft), yard (yd), mile (mi) | inch (in), foot (ft), yard (yd), mile (mi) | nautical mile (nmi)             | astronomical unit (AU), light-year, parsec |
        | Mass          | gram (g), kilogram (kg)    | ounce (oz), pound (lb), stone (st)         | ounce (oz), pound (lb)                     | (not commonly used)             | (not commonly used)                        |
        | Volume        | milliLiter (ml), liter (l) | pint (pt), quart (qt), gallon (gal)        | cup, pint (pt), quart (qt), gallon (gal)   | (not commonly used)             | (not commonly used)                        |
        | Temperature   | kelvin (K), Celsius (°C)   | Fahrenheit (°F)                            | Fahrenheit (°F)                            | (not commonly used)             | (not commonly used)                        |
            
        If you determine that the answer is using the Metric System, respond with the word "Metric".
        If you determine that the answer is using the Imperial System, respond with the word "Imperial".
        If you determine that the answer is using the US Customary Units, respond with "USCustomary".
        If you determine that the answer is using the Nautical System, respond with the word "Nautical".
        If you determine that the answer is using the Astronomical Units, respond with the word "Astronomical".
        If you determine that the answer is using more than one measurement system, respond with the word "Mixed".
        If you determine that the answer is using some other measurement system, respond with the word "Unknown".
        If you determine that the answer has nothing to do with measurements, respond with "NotApplicable".

        Note that the Imperial System and US Customary Units are very similar. If the answer is using units that
        are applicable to both systems, respond with the word "Imperial". In other words, respond with
        "USCustomary" only if the answer is using units that are unique to the US Customary Units.

        Your response should always exactly match the words or identifiers listed above. Do not include any other
        text in your response. This is important because the response is parsed using a computer program that is
        looking for the specific words or identifiers above. If you include any other text in your response, this
        computer program will not be able to understand your response.

        Answer: The bus is traveling at 60 miles per hour (mph).
        Measurement System: Imperial

        Answer: The speed of light is roughly 300,000 km/s.
        Measurement System: Metric

        Answer: The boxer weighs 70 kilograms.
        Measurement System: Metric

        Answer: The distance between the two stars is 5 light-years.
        Measurement System: Astronomical

        Answer: The distance between Jupiter and Saturn is 600 million km.
        Measurement System: Metric

        Answer: The ships is 5 nautical miles away from the port.
        Measurement System: Nautical

        Answer: The load weighed 20 stone.
        Measurement System: Imperial
        
        Answer: The distance between Mars and the asteroid belt is about 200 million miles.
        Measurement System: Imperial

        Answer: I used 2 cups of flour to make the cake.
        Measurement System: USCustomary

        Answer: The temperature outside is 25 degrees C or 77 degrees F.
        Measurement System: Mixed

        Answer: The distance between the two cities is 100 spans.
        Measurement System: Unknown

        Answer: Its raining cats and dogs.
        Measurement System: NotApplicable
        """;

    private static string GetEvaluationPrompt(string? modelResponse) =>
        $"""
        Consider the following response to a user question. What measurement system is being used in the answer?

        Answer: {modelResponse}
        Measurement System:
        """;
    #endregion

    /// <summary>
    /// Provides a default interpretation for the supplied <paramref name="metric"/>.
    /// </summary>
    /// <remarks>
    /// The default interpretation provided in this method considers the supplied <paramref name="metric"/> to be good
    /// (acceptable) if the detected measurement system is either the Imperial or US Customary. Otherwise, the
    /// <paramref name="metric"/> is considered as failed.
    /// </remarks>
    private static void Interpret(StringMetric metric)
    {
        if (string.IsNullOrWhiteSpace(metric.Value))
        {
            metric.Interpretation =
                new EvaluationMetricInterpretation(
                    EvaluationRating.Unknown,
                    failed: true,
                    reason: "Failed to detect measurement system used in the response.");
        }
        else if (Enum.TryParse(metric.Value, ignoreCase: true, out MeasurementSystem measurementSystem))
        {
            var reason = $"Detected measurement system was '{metric.Value}'.";
            metric.Interpretation =
                measurementSystem is MeasurementSystem.Imperial or MeasurementSystem.USCustomary
                    ? new EvaluationMetricInterpretation(EvaluationRating.Good, reason: reason)
                    : new EvaluationMetricInterpretation(EvaluationRating.Unacceptable, failed: true, reason);
        }
        else
        {
            metric.Interpretation =
                new EvaluationMetricInterpretation(
                    EvaluationRating.Inconclusive,
                    failed: true,
                    reason: $"The detected measurement system '{metric.Value}' is not valid.");
        }
    }

    /// <inheritdoc/>
    public async ValueTask<EvaluationResult> EvaluateAsync(
        IEnumerable<ChatMessage> messages,
        ChatMessage modelResponse,
        ChatConfiguration? chatConfiguration = null,
        IEnumerable<EvaluationContext>? additionalContext = null,
        CancellationToken cancellationToken = default)
    {
        if (chatConfiguration is null)
        {
            throw new ArgumentNullException(
                nameof(chatConfiguration),
                $"{nameof(MeasurementSystemEvaluator)} is AI-based and requires a non-null '{nameof(chatConfiguration)}'.");
        }

        var metric = new StringMetric(MeasurementSystemMetricName);

        /// Set up the prompt that we will use for performing the evaluation below.
        string evaluationPrompt = GetEvaluationPrompt(modelResponse.Text);

        if (chatConfiguration.TokenCounter is not null)
        {
            /// If an input token limit is specified as part of the <see cref="chatConfiguration"/>, check to make sure
            /// that the total number of tokens present in the <see cref="evaluationPrompt"/> and the
            /// <see cref="EvaluationSystemPrompt"/> does not exceed this limit.
            int systemPromptTokenCount = chatConfiguration.TokenCounter.CountTokens(EvaluationSystemPrompt);
            int evaluationPromptTokenCount = chatConfiguration.TokenCounter.CountTokens(evaluationPrompt);

            /// Early out with a diagnostic if the token limit is exceeded. Note that we omit setting the
            /// <see cref="StringMetric.Value"/> for the <see cref="metric"/> in this case. However, we report an error
            /// on the <see cref="metric"/> by calling
            /// <see cref="EvaluationMetric{T}.AddDiagnostic(EvaluationDiagnostic)"/>.
            if (systemPromptTokenCount + evaluationPromptTokenCount > chatConfiguration.TokenCounter.InputTokenLimit)
            {
                metric.AddDiagnostic(
                    EvaluationDiagnostic.Error(
                        $"Limit of {chatConfiguration.TokenCounter.InputTokenLimit} input tokens was exceeded."));

                return new EvaluationResult(metric);
            }
        }

        /// Invoke the LLM with the above <see cref="evaluationPrompt"/> to determine the measurement system that is
        /// used in the supplied response.
        ChatMessage[] evaluationMessages = [
            new ChatMessage(ChatRole.System, EvaluationSystemPrompt),
            new ChatMessage(ChatRole.User, evaluationPrompt)];

        ChatCompletion evaluationResponse =
            await chatConfiguration.ChatClient.CompleteAsync(
                evaluationMessages,
                new ChatOptions
                {
                    Temperature = 0.0f,
                    TopP = 1.0f,
                    PresencePenalty = 0.0f,
                    FrequencyPenalty = 0.0f,
                    ResponseFormat = ChatResponseFormat.Text
                },
                cancellationToken);

        /// Set the value of the <see cref="StringMetric"> (that we will return as part of the
        /// <see cref="EvaluationResult"/> below) to be the text of the LLM's response (which should contain the name of
        /// the detected measurement system).
        metric.Value = evaluationResponse.Message.Text;

        /// Attach a default <see cref="EvaluationMetricInterpretation"/> for the metric. An evaluator can provide a
        /// default interpretation for each metric that it produces. This default interpretation can be overridden by
        /// the caller if needed as demonstrated in
        /// <see cref="EvaluationExamples.Example06_ChangingInterpretationOfMetrics"/>.
        Interpret(metric);

        /// Return an <see cref="EvaluationResult"/> that contains the metric.
        return new EvaluationResult(metric);
    }
}
