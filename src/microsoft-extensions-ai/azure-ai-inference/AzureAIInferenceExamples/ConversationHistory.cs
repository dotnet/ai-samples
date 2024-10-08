using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using ChatRole = Microsoft.Extensions.AI.ChatRole;

public partial class AzureAIInferenceSamples
{
    public static async Task ConversationHistory() 
    {
        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient client =
            new ChatCompletionsClient(endpoint, credential)
                .AsChatClient(modelId);

        var conversation = new [] {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "What is AI?")
        };

        var response = await client.CompleteAsync(conversation);

        Console.WriteLine(response.Message);
    }    
}