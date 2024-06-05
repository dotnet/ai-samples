using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

var openAIChatCompletionModelName = "gpt-4-turbo"; // this could be other models like "gpt-4-turbo".

var builder = Kernel.CreateBuilder();

// Add logging services to the builder
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
var kernel = builder
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();

#pragma warning disable
kernel.ImportPluginFromObject(new Microsoft.SemanticKernel.Plugins.Web.WebSearchEnginePlugin(
    new BingConnector(Environment.GetEnvironmentVariable("Being_API_KEY"))));
#pragma warning disable

var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };// Set the settings for the chat completion service.
var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];

// Basic chat
while (true)
{
    Console.Write("Q: ");
    chatHistory.AddUserMessage(Console.ReadLine());// Add user message to chat history, then it can be use to get more context for the next chat response

    var response = await chatService.GetChatMessageContentsAsync(chatHistory, settings, kernel);// Get chat response based on chat history

    Console.WriteLine(response[response.Count - 1]);
    chatHistory.AddRange(response);// Add chat response to chat history, hence it can be use to get more context for the next chat response
}
