using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4o".
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];

// Basic chat
while (true)
{
    Console.Write("Q: ");
    chatHistory.AddUserMessage(Console.ReadLine()); // Add user message to chat history.
    var response = await chatService.GetChatMessageContentAsync(chatHistory); // Get chat response based on chat history.
    Console.WriteLine(response); // Print response.
    chatHistory.Add(response); // Add chat response to chat history
}
