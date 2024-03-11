// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";


// == Create the Azure Open AI Chat Completion Service ==========
AzureOpenAIChatCompletionService chatCompletionService = new(
            deploymentName: openAIDeploymentName,
            endpoint: openAIEndpoint,
            apiKey: openAiKey);


var executionSettings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 400,
    Temperature = 1f,
    TopP = 0.95f,
};

//== Read markdown file  ==========
string markdown = System.IO.File.ReadAllText("hikes.md");

// == Providing context for the AI model ==========
var systemPrompt =
"""
You are upbeat and friendly. You introduce yourself when first saying hello. 
Provide a short answer only based on the user hiking records below:  

""" + markdown;

var chatHistory = new ChatHistory(systemPrompt);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

Console.WriteLine($"\n\n\t\t-=-=- Hiking History -=-=--\n{markdown}");

// == Starting the conversation ==========



string userGreeting = """
Hi!
""";

chatHistory.AddUserMessage(userGreeting);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

chatHistory.Add(await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");


// == Providing the user's request ==========
var hikeRequest =
"""
I would like to know the ration of hike I did in Canada compare to hikes done in other countries.
""";


chatHistory.AddUserMessage(hikeRequest);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

chatHistory.Add(await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
