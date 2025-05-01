// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using OpenAI;

namespace Evaluation.Setup;

// --------------------------------------------------------------------------------------------------------------------
// NOTE: The TestSetup.GetChatConfiguration() function below is linked from the instructions in ..\..\INSTRUCTIONS.md.
// If the code in this file is modified, please ensure that the instructions (and the linked line number therein) are
// updated as well.
// --------------------------------------------------------------------------------------------------------------------

public class TestSetup
{
    private static ChatConfiguration? s_chatConfiguration;

    /// <summary>
    /// Returns an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// All the evaluations performed in the included examples will use the returned <see cref="ChatConfiguration"/> to
    /// communicate with the LLM.
    /// </remarks>
    public static ChatConfiguration GetChatConfiguration()
    {
        if (s_chatConfiguration is null)
        {
            s_chatConfiguration = GetAzureOpenAIChatConfiguration(); // Switch this to any of the below providers as needed.

            /// Note: The examples included in this solution have been primarily tested against the GPT-4o model. The
            /// prompts present within the examples directly, as well as the prompts present within the
            /// evaluators included as part of the Microsoft.Extensions.AI.Evaluation.Quality NuGet package (such as
            /// <see cref="Microsoft.Extensions.AI.Evaluation.Quality.CoherenceEvaluator"/>,
            /// <see cref="Microsoft.Extensions.AI.Evaluation.Quality.RelevanceEvaluator"/>, etc.) perform well against
            /// GPT-4o. However, they may not perform as well against other models. So, the evaluations performed in
            /// the examples may produce poor results against some other models.
            /// 
            /// That said, it can still be an interesting exercise to try out the examples against other models to
            /// understand how different models perform (i.e., how quickly or slowly are they able to perform the
            /// evaluations, how accurately can they score the coherence, relevance, fluency, etc. of the supplied
            /// responses, and so on).
        }

        return s_chatConfiguration;
    }

    private static ChatConfiguration GetAzureAIInferenceChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new ChatCompletionsClient(
                new Uri(EnvironmentVariables.AzureAIInferenceEndpoint),
                new AzureKeyCredential(EnvironmentVariables.AzureAIInferenceAPIKey))
                    .AsIChatClient(modelId: EnvironmentVariables.AzureAIInferenceModel);

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client);
    }

    private static ChatConfiguration GetAzureOpenAIChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new AzureOpenAIClient(new Uri(EnvironmentVariables.AzureOpenAIEndpoint), new DefaultAzureCredential())
                .GetChatClient(EnvironmentVariables.AzureOpenAIModel)
                .AsIChatClient();

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client);
    }

    private static ChatConfiguration GetOllamaChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new OllamaChatClient(
                new Uri(EnvironmentVariables.OllamaEndpoint),
                modelId: EnvironmentVariables.OllamaModel);

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client);
    }

    private static ChatConfiguration GetOpenAIChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new OpenAIClient(EnvironmentVariables.OpenAIAPIKey)
                .GetChatClient(EnvironmentVariables.OpenAIModel)
                .AsIChatClient();

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client);
    }
}
