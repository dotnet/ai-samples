using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task ConversationHistory() 
    {
        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId);

        var conversation = new [] {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "What is AI?")
        };

        var response = await client.CompleteAsync(conversation);

        Console.WriteLine(response.Message);
    }    
}