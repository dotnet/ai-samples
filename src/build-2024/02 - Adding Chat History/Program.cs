using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
string openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4-turbo".
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];

// Basic chat
while (true)
{
    Console.Write("Q: ");
    chatHistory.AddUserMessage(Console.ReadLine());// Add user message to chat history, then it can be use to get more context for the next chat response
    var response = await chatService.GetChatMessageContentsAsync(chatHistory);// Get chat response based on chat history
    Console.WriteLine(response[response.Count - 1]);
    chatHistory.AddRange(response);// Add chat response to chat history, hence it can be use to get more context for the next chat response
}
