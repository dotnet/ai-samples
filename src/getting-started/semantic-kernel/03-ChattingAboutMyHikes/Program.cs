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

//== Read markdown file  ==========
string markdown = System.IO.File.ReadAllText("hikes.md");

// == Providing context for the AI model ==========
var systemPrompt =
"""
You are upbeat and friendly. You introduce yourself when first saying hello. 
Provide a short answer only based on the user hiking records below:  

{{$hikingRecords}}

{{$history}}
User: {{$userInput}}
ChatBot:
""";

var chatFunction = kernel.CreateFunctionFromPrompt(systemPrompt, executionSettings);

var history = "";
var arguments = new KernelArguments()
{
    ["history"] = history,
    ["hikingRecords"] = markdown
};

Console.WriteLine($"\n\n\t\t-=-=- Hiking History -=-=--\n{markdown}");

// == Starting the conversation ==========
string userGreeting = """
Hi!
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
I would like to know the ration of hike I did in Canada compare to hikes done in other countries.
""";


arguments["userInput"] = hikeRequest;

Console.WriteLine($"\n\nUser >>> {hikeRequest}");

bot_answer = await chatFunction.InvokeAsync(kernel, arguments);
Console.WriteLine($"\n\nAssistant >>> {bot_answer}");

history += $"\nUser: {hikeRequest}\nAI: {bot_answer}\n";
arguments["history"] = history;