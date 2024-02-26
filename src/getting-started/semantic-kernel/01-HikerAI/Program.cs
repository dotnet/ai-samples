using Azure;
using Microsoft.Extensions.Configuration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Kernel = Microsoft.SemanticKernel.Kernel;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];


//Create Kernel builder
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(openAIDeploymentName,openAIEndpoint, openAiKey);

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

## User Response To Those Questions

{{$input}}
""";


var kernel = builder.Build();

var hikerAI = kernel.CreateFunctionFromPrompt(systemPrompt, executionSettings);



// == Starting the conversation ==========
string userGreeting = """
Hi! 
Apparently you can help me find a hike that I will like?
""";


Console.WriteLine($"\n\nUser >>> {userGreeting}");

var response = await kernel.InvokeAsync(hikerAI, new() { ["input"] = userGreeting });


Console.WriteLine($"\n\nAssistant >>> {response.ToString()}");



// == Providing the user's request ==========
var hikeRequest = 
"""
I live in the greater Montreal area and would like an easy hike. I don't mind driving a bit to get there.
I don't want the hike to be over 10 miles round trip. I'd consider a point-to-point hike.
I want the hike to be as isolated as possible. I don't want to see many people.
I would like it to be as bug free as possible.
""";

Console.WriteLine($"\n\nUser >>> {hikeRequest}");


var promptTemplateConfig = new PromptTemplateConfig(systemPrompt);

var promptTemplateFactory = new KernelPromptTemplateFactory();
var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);

var renderedPrompt = await promptTemplate.RenderAsync(kernel, new() { ["input"] = hikeRequest });

Console.WriteLine(renderedPrompt);




// == Retrieve the answer from HikeAI ==========
var response2 = await kernel.InvokeAsync(hikerAI, new() { ["input"] = hikeRequest });

Console.WriteLine($"\n\nAssistant >>> {response.ToString()}");
