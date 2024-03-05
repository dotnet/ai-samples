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


// == Create the Kernel ==========
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
string markdown = System.IO.File.ReadAllText("benefits.md");


// == Starting the conversation ==========
string userRequest = """
Please summarize the the following text in 20 words or less:
""" + markdown;


var chatHistory = new ChatHistory(userRequest);
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}"); ;


// == Get the response and display it ==========
chatHistory.Add(await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last().Content}");
