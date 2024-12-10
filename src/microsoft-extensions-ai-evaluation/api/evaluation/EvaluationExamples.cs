// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Setup;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;

namespace Evaluation;

[TestClass]
public partial class EvaluationExamples
{
    /// The below <see cref="ChatConfiguration"/> identifies the LLM endpoint that should be used for all evaluations
    /// performed in the current sample project. <see cref="s_chatConfiguration"/> is initialized with the value
    /// returned from <see cref="TestSetup.GetChatConfiguration"/> inside <see cref="InitializeAsync(TestContext)"/>
    /// below.
    private static ChatConfiguration? s_chatConfiguration;

    /// All unit tests in the current sample project evaluate the LLM's response to the following question: "How far is
    /// the planet Venus from the Earth at its closest and furthest points?".
    /// 
    /// We invoke the LLM once inside <see cref="InitializeAsync(TestContext)"/> below to get a response to this
    /// question and store this response in a static variable <see cref="s_response"/>. Each unit test in the current
    /// project then performs a different evaluation on the same stored response.

    private static readonly IList<ChatMessage> s_messages = [
        new ChatMessage(
            ChatRole.System,
            """
            You are an AI assistant that can answer questions related to astronomy.
            Keep your responses concise staying under 100 words as much as possible.
            Use the imperial measurement system for all measurements in your response.
            """),
        new ChatMessage(
            ChatRole.User,
            "How far is the planet Venus from the Earth at its closest and furthest points?")];

    private static ChatMessage s_response = new();

    [ClassInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        /// Set up the <see cref="ChatConfiguration"/> which includes the <see cref="IChatClient"/> that all the
        /// evaluators used in the current sample project will use to communicate with the LLM.
        s_chatConfiguration = TestSetup.GetChatConfiguration();

        var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text
            };

        /// Fetch the response to be evaluated and store it in a static variable <see cref="s_response" />.
        ChatCompletion completion = await s_chatConfiguration.ChatClient.CompleteAsync(s_messages, chatOptions);
        s_response = completion.Message;
    }
}
