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
    new OpenAIClient(key).AsChatClient(model);

// Create and print out the prompt
string prompt = $"""
    Please summarize the the following text in 20 words or less:
    {File.ReadAllText("benefits.md")}
    """;
Console.WriteLine($"user >>> {prompt}");

// Submit the prompt and print out the response
ChatCompletion response = await client.CompleteAsync(prompt, new ChatOptions { MaxOutputTokens = 400 });
Console.WriteLine($"assistant >>> {response}");
