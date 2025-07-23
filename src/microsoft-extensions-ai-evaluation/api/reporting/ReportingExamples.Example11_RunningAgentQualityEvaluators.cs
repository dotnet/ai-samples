// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Evaluation.Setup;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Reporting;

#pragma warning disable AIEVAL001
// AIEVAL001: Type is for evaluation purposes only and is subject to change or removal in future updates.
// We suppress this warning for the current file since the evaluators used in this example are marked as experimental
// at the moment.

public partial class ReportingExamples
{
    private static readonly ReportingConfiguration s_reportingConfigurationWithAgentQualityEvaluators =
        DiskBasedReportingConfiguration.Create(
            storageRootPath: EnvironmentVariables.StorageRootPath,
            evaluators:
                [new ToolCallAccuracyEvaluator(), new TaskAdherenceEvaluator(), new IntentResolutionEvaluator()],
            chatConfiguration: s_chatConfiguration,
            enableResponseCaching: true,
            executionName: ExecutionName,
            tags: GetTags());

    [TestMethod]
    public async Task Example11_RunningAgentQualityEvaluators()
    {
        /// This example demonstrates how to run agent quality evaluators (such as
        /// <see cref="ToolCallAccuracyEvaluator"/>, <see cref="TaskAdherenceEvaluator"/>, and
        /// <see cref="IntentResolutionEvaluator"/>) that assess how well an AI agent performs tasks involving tool use
        /// and conversational interactions.

        await using ScenarioRun scenarioRun =
            await s_reportingConfigurationWithAgentQualityEvaluators.CreateScenarioRunAsync(
                this.ScenarioName,
                additionalTags: ["Order Status"]);

        IChatClient chatClient = scenarioRun.ChatConfiguration!.ChatClient;

        /// Get a conversation that simulates a customer service agent using tools to assist a customer.
        (IList<ChatMessage> messages, ChatResponse response, IList<AITool> toolDefinitions) =
            await GetConversationWithToolsAsync(chatClient);

        /// The agent quality evaluators require tool definitions to assess tool-related behaviors.
        var toolCallAccuracyContext = new ToolCallAccuracyEvaluatorContext(toolDefinitions);
        var taskAdherenceContext = new TaskAdherenceEvaluatorContext(toolDefinitions);
        var intentResolutionContext = new IntentResolutionEvaluatorContext(toolDefinitions);

        /// Run the agent quality evaluators against the response.
        EvaluationResult result =
            await scenarioRun.EvaluateAsync(
                messages,
                response,
                additionalContext: [toolCallAccuracyContext, taskAdherenceContext, intentResolutionContext]);

        /// Run some basic validation on the evaluation result.
        ValidateAgentQuality(result);
    }

    private static async Task<(IList<ChatMessage> Messages, ChatResponse ModelResponse, IList<AITool> ToolDefinitions)>
        GetConversationWithToolsAsync(IChatClient chatClient)
    {
        const string SystemPrompt =
            """
            You are a helpful customer service agent.
            Use the available tools to assist customers with their order inquiries.
            """;

        const string CustomerQuery =
            """
            Hi, I need help with the last 2 orders on my account #888.
            Could you please update me on their status?
            """;

        List<ChatMessage> messages =
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                new ChatMessage(ChatRole.User, CustomerQuery)
            ];

        List<AITool> toolDefinitions =
            [
                AIFunctionFactory.Create(GetOrders),
                AIFunctionFactory.Create(GetOrderStatus)
            ];

        var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text,
                Tools = toolDefinitions
            };

        /// Get an <see cref="IChatClient"/> with function calling enabled.
        chatClient = chatClient.AsBuilder().UseFunctionInvocation().Build();

        ChatResponse response = await chatClient.GetResponseAsync(messages, chatOptions);
        return (messages, response, toolDefinitions);
    }

    private static void ValidateAgentQuality(EvaluationResult result)
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

        /// Retrieve the score for tool call accuracy from the <see cref="EvaluationResult"/>.
        BooleanMetric toolCallAccuracy = result.Get<BooleanMetric>(ToolCallAccuracyEvaluator.ToolCallAccuracyMetricName);
        toolCallAccuracy.Interpretation!.Failed.Should().BeFalse(because: toolCallAccuracy.Interpretation.Reason);
        toolCallAccuracy.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: toolCallAccuracy.Reason);
        toolCallAccuracy.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        toolCallAccuracy.Value.Should().BeTrue(because: toolCallAccuracy.Reason);

        /// Retrieve the score for task adherence from the <see cref="EvaluationResult"/>.
        NumericMetric taskAdherence = result.Get<NumericMetric>(TaskAdherenceEvaluator.TaskAdherenceMetricName);
        taskAdherence.Interpretation!.Failed.Should().BeFalse(because: taskAdherence.Interpretation.Reason);
        taskAdherence.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: taskAdherence.Reason);
        taskAdherence.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        taskAdherence.Value.Should().BeGreaterThanOrEqualTo(4, because: taskAdherence.Reason);

        /// Retrieve the score for intent resolution from the <see cref="EvaluationResult"/>.
        NumericMetric intentResolution = result.Get<NumericMetric>(IntentResolutionEvaluator.IntentResolutionMetricName);
        intentResolution.Interpretation!.Failed.Should().BeFalse(because: intentResolution.Interpretation.Reason);
        intentResolution.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: intentResolution.Reason);
        intentResolution.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        intentResolution.Value.Should().BeGreaterThanOrEqualTo(4, because: intentResolution.Reason);
    }

    #region Tools
    private record CustomerOrder(int OrderId);
    private record CustomerOrderStatus(int OrderId, string Status, DateTime ExpectedDelivery);

    /// <summary>
    /// Mock function to get customer orders.
    /// </summary>
    [Description("Gets the orders for a customer")]
    private static IReadOnlyList<CustomerOrder> GetOrders(
        [Description("The customer account number")] int accountNumber)
    {
        return accountNumber switch
        {
            888 => [new CustomerOrder(123), new CustomerOrder(124)],
            _ => throw new InvalidOperationException($"Account number {accountNumber} is not valid.")
        };
    }

    /// <summary>
    /// Mock function to get order delivery status.
    /// </summary>
    [Description("Gets the delivery status of an order")]
    private static CustomerOrderStatus GetOrderStatus(
        [Description("The order ID to check")] int orderId)
    {
        return orderId switch
        {
            123 => new CustomerOrderStatus(orderId, "shipped", DateTime.Now.AddDays(1)),
            124 => new CustomerOrderStatus(orderId, "delayed", DateTime.Now.AddDays(10)),
            _ => throw new InvalidOperationException($"Order with ID {orderId} not found.")
        };
    }
    #endregion
}
