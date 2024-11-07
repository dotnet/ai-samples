// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

// Retrieve the local secrets that were set from the command line, using:
// dotnet user-secrets init
// dotnet user-secrets set OpenAIKey <your-openai-key>
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string model = "gpt-3.5-turbo";
string key = config["OpenAIKey"];

// Create the IChatClient
IChatClient client =
    new OpenAIClient(key)
        .AsChatClient(model);

// Provide context for the AI model
List<ChatMessage> chatHistory = [new(ChatRole.System, $"""
    You are upbeat and friendly. You introduce yourself when first saying hello. 
    Provide a short answer only based on the user hiking records below:  

    {File.ReadAllText("hikes.md")}
    """)];
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

// Start the conversation
chatHistory.Add(new ChatMessage(ChatRole.User, "Hi!"));
await PrintAndSendAsync();

// Continue the conversation with a question.
chatHistory.Add(new ChatMessage(ChatRole.User, "I would like to know the ratio of the hikes I've done in Canada compared to other countries."));
await PrintAndSendAsync();

async Task PrintAndSendAsync()
{
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");
    var response = await client.CompleteAsync(chatHistory, new ChatOptions { MaxOutputTokens = 400 });
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");
}
