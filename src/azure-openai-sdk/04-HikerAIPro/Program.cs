// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];

// == Creating the AIClient ==========
var endpoint = new Uri(openAIEndpoint);
var credentials = new AzureKeyCredential(openAiKey);
var openAIClient = new OpenAIClient(endpoint, credentials);


// == Defining a Tool to extend the AI model ==========
var getWeather = new ChatCompletionsFunctionToolDefinition()
{
    Name = "get_current_weather",
    Description = "Get the current weather in a given location",
    Parameters = BinaryData.FromObjectAsJson(
    new
    {
        Type = "object",
        Properties = new
        {
            Location = new
            {
                Type = "string",
                Description = "The city, e.g. Montreal, Sidney",
            },
            Unit = new
            {
                Type = "string",
                Enum = new[] { "celsius", "fahrenheit" },
            }
        },
        Required = new[] { "location" },
    },
    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
};

// == Purely for convenience and clarity, this standalone local method handles tool call responses. ==========
ChatRequestToolMessage GetToolCallResponseMessage(ChatCompletionsToolCall toolCall)
{
    var functionToolCall = toolCall as ChatCompletionsFunctionToolCall;
    if (functionToolCall?.Name == getWeather.Name)
    {
        string unvalidatedArguments = functionToolCall.Arguments;
        var functionResultData = (object)null;

        // == Here you would call a weather API to get the weather for the location ==========
        functionResultData = "Periods of rain or drizzle, 15 C";

        return new ChatRequestToolMessage(functionResultData.ToString(), toolCall.Id);
    }
    else
    {
        throw new NotImplementedException();
    }
}


var completionOptions = new ChatCompletionsOptions
{
    MaxTokens = 400,
    Temperature = 1f,
    FrequencyPenalty = 0.0f,
    PresencePenalty = 0.0f,
    NucleusSamplingFactor = 0.95f, // Top P
    DeploymentName = openAIDeploymentName,
    Tools = { getWeather }
};
completionOptions.ToolChoice = ChatCompletionsToolChoice.Auto;


// == Providing context for the AI model ==========
var systemPrompt =
"""
You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly.
A good weather is important for a good hike. Only make recommendations if the weather is good or if people insist.
You introduce yourself when first saying hello. When helping people out, you always ask them 
for this information to inform the hiking recommendation you provide:

1. Where they are located
2. What hiking intensity they are looking for

You will then provide three suggestions for nearby hikes that vary in length after you get that information. 
You will also share an interesting fact about the local nature on the hikes when making a recommendation.
""";

completionOptions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));


// == Starting the conversation ==========
string userGreeting = """
Hi! 
""";

completionOptions.Messages.Add(new ChatRequestUserMessage(userGreeting));
Console.WriteLine($"\n\nUser >>> {userGreeting}");


ChatCompletions response = await openAIClient.GetChatCompletionsAsync(completionOptions);
ChatResponseMessage assistantResponse = response.Choices[0].Message;
Console.WriteLine($"\n\nAssistant >>> {assistantResponse.Content}");
completionOptions.Messages.Add(new ChatRequestSystemMessage(assistantResponse.Content));


// == Providing the user's request ==========
var hikeRequest =
"""
Is the weather is good today for a hike?
If yes, I live in the greater Montreal area and would like an easy hike. I don't mind driving a bit to get there.
I don't want the hike to be over 10 miles round trip. I'd consider a point-to-point hike.
I want the hike to be as isolated as possible. I don't want to see many people.
I would like it to be as bug free as possible.
""";

Console.WriteLine($"\n\nUser >>> {hikeRequest}");
completionOptions.Messages.Add(new ChatRequestUserMessage(hikeRequest));

// == Retrieve the answer from HikeAI ==========
response = await openAIClient.GetChatCompletionsAsync(completionOptions);

// == If the response includes a tool call, handle it and continue the conversation ==========
ChatChoice responseChoice = response.Choices[0];
if (responseChoice.FinishReason == CompletionsFinishReason.ToolCalls)
{
    // == Include the FunctionCall message in the conversation history ==========
    completionOptions.Messages.Add(new ChatRequestAssistantMessage(responseChoice.Message));

    // == Add a new tool message for each tool call that is resolved ==========
    foreach (ChatCompletionsToolCall toolCall in responseChoice.Message.ToolCalls)
    {
        var ToolCallMsg = GetToolCallResponseMessage(toolCall);
        completionOptions.Messages.Add(ToolCallMsg);
    }

    // == Retrieve the answer from HikeAI Pro ==========
    response = await openAIClient.GetChatCompletionsAsync(completionOptions);
}

assistantResponse = response.Choices[0].Message;
Console.WriteLine($"\n\nAssistant >>> {assistantResponse.Content}");
