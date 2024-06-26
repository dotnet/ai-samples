// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Retrieve the local secrets that were set from the command line, using:
// dotnet user-secrets init
// dotnet user-secrets set OpenAIKey <your-openai-key>
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string model = "gpt-3.5-turbo";
string key = config["OpenAIKey"];

// Create the OpenAI Chat Completion Service
OpenAIChatCompletionService service = new(model, key);

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
