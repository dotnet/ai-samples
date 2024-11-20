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

// Start the conversation with context for the AI model
ChatHistory chatHistory = new("""
    You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly. 
    You introduce yourself when first saying hello. When helping people out, you always ask them 
    for this information to inform the hiking recommendation you provide:

    1. Where they are located
    2. What hiking intensity they are looking for

    You will then provide three suggestions for nearby hikes that vary in length after you get that information. 
    You will also share an interesting fact about the local nature on the hikes when making a recommendation.
    """);
await PrintAndSendAsync();

// Continue the conversation with a question
chatHistory.AddUserMessage("Hi! Apparently you can help me find a hike that I will like?");
await PrintAndSendAsync();

// Continue the conversation with another question
chatHistory.AddUserMessage("""
    I live in the greater Montreal area and would like an easy hike. I don't mind driving a bit to get there.
    I don't want the hike to be over 10 miles round trip. I'd consider a point-to-point hike.
    I want the hike to be as isolated as possible. I don't want to see many people.
    I would like it to be as bug free as possible.
    """);
await PrintAndSendAsync();

async Task PrintAndSendAsync()
{
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
    chatHistory.Add(await service.GetChatMessageContentAsync(chatHistory, new OpenAIPromptExecutionSettings() { MaxTokens = 400 }));
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
}
