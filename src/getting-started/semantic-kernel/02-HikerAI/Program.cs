using Microsoft.Extensions.Configuration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Kernel = Microsoft.SemanticKernel.Kernel;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";


// == Create the Kernel ==========
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(openAIDeploymentName, openAIEndpoint, openAiKey);

var executionSettings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 400,
    Temperature = 1f,
    TopP = 0.95f,
};

var kernel = builder.Build();

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

{{$history}}
User: {{$userInput}}
ChatBot:";

""";


var chatFunction = kernel.CreateFunctionFromPrompt(systemPrompt, executionSettings);

var history = "";
var arguments = new KernelArguments()
{
    ["history"] = history
};

// == Starting the conversation ==========
string userGreeting = """
Hi! 
Apparently you can help me find a hike that I will like?
""";


arguments["userInput"] = userGreeting;

Console.WriteLine($"\n\nUser >>> {userGreeting}");
var bot_answer = await chatFunction.InvokeAsync(kernel, arguments);

Console.WriteLine($"\n\nAssistant >>> {bot_answer}");

history += $"\nUser: {userGreeting}\nAI: {bot_answer}\n";
arguments["history"] = history;

// == Providing the user's request ==========
var hikeRequest = 
"""
I live in the greater Montreal area and would like an easy hike. I don't mind driving a bit to get there.
I don't want the hike to be over 10 miles round trip. I'd consider a point-to-point hike.
I want the hike to be as isolated as possible. I don't want to see many people.
I would like it to be as bug free as possible.
""";


arguments["userInput"] = hikeRequest;
Console.WriteLine($"\n\nUser >>> {hikeRequest}");


// == Retrieve the answer from HikeAI ==========
bot_answer = await chatFunction.InvokeAsync(kernel, arguments);
Console.WriteLine($"\n\nAssistant >>> {bot_answer}");
