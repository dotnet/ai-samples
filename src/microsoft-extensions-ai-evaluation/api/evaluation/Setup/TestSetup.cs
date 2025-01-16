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
        IChatClient client =
            new ChatCompletionsClient(
                new Uri(EnvironmentVariables.AzureAIInferenceEndpoint),
                new AzureKeyCredential(EnvironmentVariables.AzureAIInferenceAPIKey))
                    .AsChatClient(modelId: EnvironmentVariables.AzureAIInferenceModel);

        IEvaluationTokenCounter tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.AzureAIInferenceModel)
                .ToTokenCounter(EnvironmentVariables.AzureAIInferenceModelInputTokenLimit);

        return new ChatConfiguration(client, tokenCounter);
    }

    private static ChatConfiguration GetAzureOpenAIChatConfiguration()
    {
        IChatClient client =
            new AzureOpenAIClient(new Uri(EnvironmentVariables.AzureOpenAIEndpoint), new DefaultAzureCredential())
                .AsChatClient(modelId: EnvironmentVariables.AzureOpenAIModel);

        IEvaluationTokenCounter tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.AzureOpenAIModel)
                .ToTokenCounter(EnvironmentVariables.AzureOpenAIModelInputTokenLimit);

        return new ChatConfiguration(client, tokenCounter);
    }

    private static ChatConfiguration GetOllamaChatConfiguration()
    {
        IChatClient client = new OllamaChatClient(new Uri(EnvironmentVariables.OllamaEndpoint));

        IEvaluationTokenCounter tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.OllamaModel)
                .ToTokenCounter(EnvironmentVariables.OllamaModelInputTokenLimit);

        return new ChatConfiguration(client, tokenCounter);
    }

    private static ChatConfiguration GetOpenAIChatConfiguration()
    {
        IChatClient client =
            new OpenAIClient(EnvironmentVariables.OpenAIAPIKey)
                .AsChatClient(modelId: EnvironmentVariables.OpenAIModel);

        IEvaluationTokenCounter tokenCounter =
            TiktokenTokenizer.CreateForModel(EnvironmentVariables.OpenAIModel)
                .ToTokenCounter(EnvironmentVariables.OpenAIModelInputTokenLimit);

        return new ChatConfiguration(client, tokenCounter);
    }
}
