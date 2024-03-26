// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Retrieve the local secrets saved during the Azure deployment. If you skipped the deployment
// because you already have an Azure OpenAI available, edit the following lines to use your information,
// e.g. string openAIEndpoint = "https://cog-demo123.openai.azure.com/";
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["AZURE_OPENAI_GPT_ENDPOINT"];
string deployment = config["AZURE_OPENAI_GPT_NAME"];
string key = config["AZURE_OPENAI_GPT_KEY"];

// Create a Kernel containing the Azure OpenAI Chat Completion Service
IKernelBuilder b = Kernel.CreateBuilder();
b.Services.ConfigureHttpClientDefaults(c => c.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromSeconds(500.0),
        Name = "Standard-AttemptTimeout"
    };
}));
b.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace)); // uncomment to see all interactions with the model logged

Kernel kernel = b
    .AddAzureOpenAIChatCompletion(deployment, endpoint, key)
    .Build();

// Add a new plugin with a local .NET function that should be available to the AI model
kernel.ImportPluginFromFunctions("WeatherPlugin",
[
    KernelFunctionFactory.CreateFromMethod(([Description("The city, e.g. Montreal, Sidney")] string location, string unit = null) =>
    {
        // Here you would call a weather API to get the weather for the location
        return "Periods of rain or drizzle, 15 C";
    }, "get_current_weather", "Get the current weather in a given location")
]);

// Start the conversation
ChatHistory chatHistory = new("""
    You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly.
    You introduce yourself when first saying hello.
    """);
chatHistory.AddUserMessage("Hi!");
await PrintAndSendAsync();

// Continue the conversation
chatHistory.AddUserMessage("I live in Montreal and I'm looking for a moderate intensity hike. Is the weather good today for a hike? ");
await PrintAndSendAsync();

async Task PrintAndSendAsync()
{
    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

    OpenAIPromptExecutionSettings settings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
    chatHistory.Add(await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(chatHistory, settings, kernel));

    Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
}
