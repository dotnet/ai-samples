// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

// Retrieve the local secrets that were set from the command line, using:
// dotnet user-secrets init
// dotnet user-secrets set OpenAIKey <your-openai-key>
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string model = "gpt-3.5-turbo";
string key = config["OpenAIKey"];

// Create the IChatClient
IChatClient client =
    new ChatClientBuilder()
        .UseFunctionInvocation()
        .Use(new OpenAIClient(key).AsChatClient(model));

// Add a new plugin with a local .NET function that should be available to the AI model
var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create((string location, string unit) =>
    {
        // Here you would call a weather API to get the weather for the location
        return "Periods of rain or drizzle, 15 C";
    },
    "get_current_weather",
    "Get the current weather in a given location")]
};


// Start the conversation
List<ChatMessage> chatHistory = [new(ChatRole.System, """
    You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly.
    You introduce yourself when first saying hello.
    """)];

chatHistory.Add(new ChatMessage(ChatRole.User, "Hi!"));
await PrintAndSendAsync();

// Continue the conversation
chatHistory.Add(new ChatMessage(ChatRole.User, "I live in Montreal and I'm looking for a moderate intensity hike. Is the weather good today for a hike? "));
await PrintAndSendAsync();

async Task PrintAndSendAsync()
{
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");
    var response = await client.CompleteAsync(chatHistory, chatOptions);
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Message.Contents));
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");
}
