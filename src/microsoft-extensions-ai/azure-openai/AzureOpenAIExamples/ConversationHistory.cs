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

        List<ChatMessage> conversation =
        [
            new(ChatRole.System, "You are a helpful AI assistant"),
            new(ChatRole.User, "What is AI?")
        ];

        Console.WriteLine(await client.CompleteAsync(conversation));
    }    
}
