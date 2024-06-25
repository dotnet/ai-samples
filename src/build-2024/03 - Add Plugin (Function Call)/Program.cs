using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4o".

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();

// Import the DemographicInfo class to the kernel, so it can be used in the chat completion service.
// this plugin could be from other options such as functions, prompts directory, etc.
kernel.ImportPluginFromType<DemographicInfo>();
var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };// Set the settings for the chat completion service.
var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];

// Basic chat
while (true)
{
    Console.Write("Q: ");
    chatHistory.AddUserMessage(Console.ReadLine());// Add user message to chat history, then it can be use to get more context for the next chat response

    var response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);// Get chat response based on chat history

    Console.WriteLine(response);
    chatHistory.Add(response);// Add chat response to chat history, hence it can be use to get more context for the next chat response
}

class DemographicInfo
{
    [KernelFunction]
    public int GetAge(string name)
    {
        return name switch
        {
            "Alice" => 25,
            "Bob" => 30,
            _ => 0
        };
    }
}
