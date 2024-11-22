using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task ConversationHistory() 
    {
        var endpoint = "http://localhost:11434/";
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId);

        List<ChatMessage> conversation =
        [
            new(ChatRole.System, "You are a helpful AI assistant"),
            new(ChatRole.User, "What is AI?")
        ];

        Console.WriteLine(await client.CompleteAsync(conversation));
    }    
}
