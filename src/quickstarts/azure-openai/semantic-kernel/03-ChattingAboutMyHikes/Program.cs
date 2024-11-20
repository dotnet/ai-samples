// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Retrieve the local secrets saved during the Azure deployment. If you skipped the deployment
// because you already have an Azure OpenAI available, edit the following lines to use your information,
// e.g. string openAIEndpoint = "https://cog-demo123.openai.azure.com/";
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["AZURE_OPENAI_ENDPOINT"];
string deployment = config["AZURE_OPENAI_GPT_NAME"];

// Create the Azure OpenAI Chat Completion Service
AzureOpenAIChatCompletionService service = new(deployment, endpoint, new DefaultAzureCredential());

// Provide context for the AI model
ChatHistory chatHistory = new($"""
    You are upbeat and friendly. You introduce yourself when first saying hello. 
    Provide a short answer only based on the user hiking records below:  

    {File.ReadAllText("hikes.md")}
    """);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

// Start the conversation
chatHistory.AddUserMessage("Hi!");
await PrintAndSendAsync();

// Continue the conversation with a question.
chatHistory.AddUserMessage("I would like to know the ratio of the hikes I've done in Canada compared to other countries.");
await PrintAndSendAsync();

async Task PrintAndSendAsync()
{
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
    chatHistory.Add(await service.GetChatMessageContentAsync(chatHistory, new OpenAIPromptExecutionSettings() { MaxTokens = 400 }));
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
}
