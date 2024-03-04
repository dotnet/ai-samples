using Microsoft.Extensions.Configuration;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;
using Kernel = Microsoft.SemanticKernel.Kernel;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];
string openAIDalleName = config["AZURE_OPENAI_DALLE_NAME"];
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";


// == Create the Kernel ==========
var builder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0012 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.AddAzureOpenAITextToImage(openAIDalleName, openAIEndpoint, openAiKey);
#pragma warning restore SKEXP0012 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.AddAzureOpenAIChatCompletion(openAIDeploymentName, openAIEndpoint, openAiKey);

var executionSettings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 400,
    Temperature = 1f,
    TopP = 0.95f,
};

var kernel = builder.Build();



// == Define the image ==========
string imagePrompt = """
A postal card with an happy hiker waving, there a beautiful mountain in the background.
There is a trail visible in the foreground. 
The postal card has text in red saying: 'You are invited for a hike!'
""";


#pragma warning disable SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var dallE = kernel.GetRequiredService<ITextToImageService>();
#pragma warning restore SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var generateImage = kernel.CreateFunctionFromPrompt(imagePrompt, executionSettings);

var imageDescResult = await kernel.InvokeAsync(generateImage, new() { });
var imageDesc = imageDescResult.ToString();

#pragma warning disable SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var imageUrl = await dallE.GenerateImageAsync(imageDesc.Trim(), 1024, 1024);
#pragma warning restore SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine($"\n\nThe generated image is ready at:\n {imageUrl}");