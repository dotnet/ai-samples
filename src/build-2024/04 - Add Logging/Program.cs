using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4o".

var builder = Kernel.CreateBuilder();

// Add logging services to the builder
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

var kernel = builder
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];

// Basic chat
while (true)
{
    Console.Write("Q: ");
    chatHistory.AddUserMessage(Console.ReadLine());// Add user message to chat history, then it can be use to get more context for the next chat response

    var response = await chatService.GetChatMessageContentAsync(chatHistory);// Get chat response based on chat history

    Console.WriteLine(response);
    chatHistory.Add(response);// Add chat response to chat history, hence it can be use to get more context for the next chat response
}
