// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

// Retrieve the local secrets saved during the Azure deployment. If you skipped the deployment
// because you already have an Azure OpenAI available, edit the following lines to use your information,
// e.g. string openAIEndpoint = "https://cog-demo123.openai.azure.com/";
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["AZURE_OPENAI_ENDPOINT"];
string deployment = config["AZURE_OPENAI_GPT_NAME"];
string key = config["AZURE_OPENAI_KEY"];

// Create the IChatClient
IChatClient client =
    new ChatClientBuilder()
        .UseFunctionInvocation()
        .Use(
            new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key))
                .AsChatClient(deployment));

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