using OpenAI;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;

public partial class OpenAISamples
{
    public static async Task ConversationHistory() 
    {
        IChatClient client =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                .AsChatClient(modelId: "gpt-4o-mini");

        var conversation = new [] {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "What is AI?")
        };

        var response = await client.CompleteAsync(conversation);

        Console.WriteLine(response.Message);
    }    
}