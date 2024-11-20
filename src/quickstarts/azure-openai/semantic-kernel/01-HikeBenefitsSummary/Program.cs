// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Retrieve the local secrets saved during the Azure deployment. If you skipped the deployment
// because you already have an Azure OpenAI available, edit the following lines to use your information,
// e.g. string openAIEndpoint = "https://cog-demo123.openai.azure.com/";
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["AZURE_OPENAI_ENDPOINT"];
string deployment = config["AZURE_OPENAI_GPT_NAME"];

// Create a Kernel containing the Azure OpenAI Chat Completion Service
Kernel kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deployment, endpoint, new DefaultAzureCredential())
    .Build();

// Create and print out the prompt
string prompt = $"""
    Please summarize the the following text in 20 words or less:
    {File.ReadAllText("benefits.md")}
    """;
Console.WriteLine($"user >>> {prompt}");

// Submit the prompt and print out the response
string response = await kernel.InvokePromptAsync<string>(prompt, new(new OpenAIPromptExecutionSettings() { MaxTokens = 400 }));
Console.WriteLine($"assistant >>> {response}");
