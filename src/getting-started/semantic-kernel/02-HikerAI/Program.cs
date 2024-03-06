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

// == Providing context for the AI model ==========
var systemPrompt =
"""
You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly. 
You introduce yourself when first saying hello. When helping people out, you always ask them 
for this information to inform the hiking recommendation you provide:

1. Where they are located
2. What hiking intensity they are looking for

You will then provide three suggestions for nearby hikes that vary in length after you get that information. 
You will also share an interesting fact about the local nature on the hikes when making a recommendation.
""";


var chatHistory = new ChatHistory(systemPrompt);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");


// == Get the response and display it ==========
chatHistory.Add(await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

// == Starting the conversation ==========
string userGreeting = """
Hi! 
Apparently you can help me find a hike that I will like?
""";


chatHistory.AddUserMessage(userGreeting);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");

chatHistory.Add(await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");


// == Providing the user's request ==========
var hikeRequest =
"""
I live in the greater Montreal area and would like an easy hike. I don't mind driving a bit to get there.
I don't want the hike to be over 10 miles round trip. I'd consider a point-to-point hike.
I want the hike to be as isolated as possible. I don't want to see many people.
I would like it to be as bug free as possible.
""";

chatHistory.AddUserMessage(hikeRequest);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");


// == Retrieve the answer from HikeAI ==========
chatHistory.Add(await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
