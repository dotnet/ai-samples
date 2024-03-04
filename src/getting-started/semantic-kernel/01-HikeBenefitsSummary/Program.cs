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
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";


// == Creating the AIClient ==========

//Create Kernel builder
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
string markdown = File.ReadAllText("benefits.md");


// == Starting the conversation ==========
string userRequest = """
Please summarize the the following text in 20 words or less:

{{$input}}
""";



var promptTemplateConfig = new PromptTemplateConfig(userRequest);

var promptTemplateFactory = new KernelPromptTemplateFactory();
var promptTemplate = promptTemplateFactory.Create(promptTemplateConfig);

var renderedPrompt = await promptTemplate.RenderAsync(kernel, new() { ["input"] = markdown });

Console.WriteLine($"\n\nUser >>> {renderedPrompt}");


var summaryFunction = kernel.CreateFunctionFromPrompt(userRequest, executionSettings);


// == Get the response and display it ==========
var result = await kernel.InvokeAsync(summaryFunction, new() { ["input"] = markdown });

Console.WriteLine($"\n\nAssistant >>> {result}");


