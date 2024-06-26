// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";


// == Creating the AIClient ==========
var endpoint = new Uri(openAIEndpoint);
var credentials = new AzureKeyCredential(openAiKey);
var openAIClient = new OpenAIClient(endpoint, credentials);

var completionOptions = new ChatCompletionsOptions
{
    MaxTokens = 1000,
    Temperature = 1f,
    FrequencyPenalty = 0.0f,
    PresencePenalty = 0.0f,
    NucleusSamplingFactor = 0.95f, // Top P
    DeploymentName = openAIDeploymentName
};

//== Read markdown file  ==========
string markdown = System.IO.File.ReadAllText("hikes.md");

// == Providing context for the AI model ==========
var systemPrompt =
"""
You are upbeat and friendly. You introduce yourself when first saying hello. 
Provide a short answer only based on the user hiking records below:  

""" + markdown;
completionOptions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));

Console.WriteLine($"\n\n\t\t-=-=- Hiking History -=-=--\n{markdown}");

// == Starting the conversation ==========
string userGreeting = """
Hi!
""";

completionOptions.Messages.Add(new ChatRequestUserMessage(userGreeting));
Console.WriteLine($"\n\nUser >>> {userGreeting}");

ChatCompletions response = await openAIClient.GetChatCompletionsAsync(completionOptions);
ChatResponseMessage assistantResponse = response.Choices[0].Message;
Console.WriteLine($"\n\nAssistant >>> {assistantResponse.Content}");
completionOptions.Messages.Add(new ChatRequestAssistantMessage(assistantResponse.Content));


// == Providing the user's request ==========
var hikeRequest =
"""
I would like to know the ration of hike I did in Canada compare to hikes done in other countries.
""";


Console.WriteLine($"\n\nUser >>> {hikeRequest}");
completionOptions.Messages.Add(new ChatRequestUserMessage(hikeRequest));

// == Retrieve the answer from HikeAI ==========
response = await openAIClient.GetChatCompletionsAsync(completionOptions);
assistantResponse = response.Choices[0].Message;

Console.WriteLine($"\n\nAssistant >>> {assistantResponse.Content}");
