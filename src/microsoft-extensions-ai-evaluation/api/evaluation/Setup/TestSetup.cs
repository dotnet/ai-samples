// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.ML.Tokenizers;
using OpenAI;

namespace Evaluation.Setup;

public class TestSetup
{
    private static ChatConfiguration? s_chatConfiguration;

    public static ChatConfiguration GetChatConfiguration()
    {
        if (s_chatConfiguration is null)
        {
            s_chatConfiguration = GetAzureOpenAIChatConfiguration(); // Switch this to any of the below providers as needed.
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
                    .AsChatClient(modelId: EnvironmentVariables.AzureAIInferenceModel);

        IEvaluationTokenCounter? tokenCounter = null;
        if (EnvironmentVariables.AzureAIInferenceModelInputTokenLimit.HasValue)
        {
            /// Note that while <see cref="TiktokenTokenizer"/> supports a wide variety of models, it may not (yet)
            /// support the model you have selected. In this case, you can either turn off the token counting for all
            /// evaluations performed in the included examples (by unsetting the above environment variable that
            /// specifies the token limit), or you can implement <see cref="IEvaluationTokenCounter"/> for the selected
            /// model (by wrapping any other tokenizer API that that supports the selected model) and pass this
            /// <see cref="IEvaluationTokenCounter"/> down to the <see cref="ChatConfiguration"/> created below.
            tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.AzureAIInferenceModel)
                    .ToTokenCounter(EnvironmentVariables.AzureAIInferenceModelInputTokenLimit.Value);
        }

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client, tokenCounter);
    }

    private static ChatConfiguration GetAzureOpenAIChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new AzureOpenAIClient(new Uri(EnvironmentVariables.AzureOpenAIEndpoint), new DefaultAzureCredential())
                .AsChatClient(modelId: EnvironmentVariables.AzureOpenAIModel);

        IEvaluationTokenCounter? tokenCounter = null;
        if (EnvironmentVariables.AzureOpenAIModelInputTokenLimit.HasValue)
        {
            /// Note that while <see cref="TiktokenTokenizer"/> supports a wide variety of models, it may not (yet)
            /// support the model you have selected. In this case, you can either turn off the token counting for all
            /// evaluations performed in the included examples (by unsetting the above environment variable that
            /// specifies the token limit), or you can implement <see cref="IEvaluationTokenCounter"/> for the selected
            /// model (by wrapping any other tokenizer API that that supports the selected model) and pass this
            /// <see cref="IEvaluationTokenCounter"/> down to the <see cref="ChatConfiguration"/> created below.
            tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.AzureOpenAIModel)
                    .ToTokenCounter(EnvironmentVariables.AzureOpenAIModelInputTokenLimit.Value);
        }

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client, tokenCounter);
    }

    private static ChatConfiguration GetOllamaChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new OllamaChatClient(
                new Uri(EnvironmentVariables.OllamaEndpoint),
                modelId: EnvironmentVariables.OllamaModel);

        IEvaluationTokenCounter? tokenCounter = null;
        if (EnvironmentVariables.OllamaModelInputTokenLimit.HasValue)
        {
            /// Note that while <see cref="TiktokenTokenizer"/> supports a wide variety of models, it may not (yet)
            /// support the model you have selected. In this case, you can either turn off the token counting for all
            /// evaluations performed in the included examples (by unsetting the above environment variable that
            /// specifies the token limit), or you can implement <see cref="IEvaluationTokenCounter"/> for the selected
            /// model (by wrapping any other tokenizer API that that supports the selected model) and pass this
            /// <see cref="IEvaluationTokenCounter"/> down to the <see cref="ChatConfiguration"/> created below.
            tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.OllamaModel)
                    .ToTokenCounter(EnvironmentVariables.OllamaModelInputTokenLimit.Value);
        }

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client, tokenCounter);
    }

    private static ChatConfiguration GetOpenAIChatConfiguration()
    {
        /// Get an instance of Microsoft.Extensions.AI's <see cref="IChatClient"/> interface for the selected LLM
        /// endpoint.
        IChatClient client =
            new OpenAIClient(EnvironmentVariables.OpenAIAPIKey)
                .AsChatClient(modelId: EnvironmentVariables.OpenAIModel);

        IEvaluationTokenCounter? tokenCounter = null;
        if (EnvironmentVariables.OpenAIModelInputTokenLimit.HasValue)
        {
            /// Note that while <see cref="TiktokenTokenizer"/> supports a wide variety of models, it may not (yet)
            /// support the model you have selected. In this case, you can either turn off the token counting for all
            /// evaluations performed in the included examples (by unsetting the above environment variable that
            /// specifies the token limit), or you can implement <see cref="IEvaluationTokenCounter"/> for the selected
            /// model (by wrapping any other tokenizer API that that supports the selected model) and pass this
            /// <see cref="IEvaluationTokenCounter"/> down to the <see cref="ChatConfiguration"/> created below.
            tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.OpenAIModel)
                    .ToTokenCounter(EnvironmentVariables.OpenAIModelInputTokenLimit.Value);
        }

        /// Create an instance of Microsoft.Extensions.AI.Evaluation's <see cref="ChatConfiguration"/>. All the
        /// evaluations performed in the included examples will use this <see cref="ChatConfiguration"/> to communicate
        /// with the LLM.
        return new ChatConfiguration(client, tokenCounter);
    }
}
