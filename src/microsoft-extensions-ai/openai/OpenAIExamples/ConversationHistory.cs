using OpenAI;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task ConversationHistory() 
    {
        IChatClient client =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsChatClient("gpt-4o-mini");

        var conversation = new [] {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "What is AI?")
        };

        var response = await client.CompleteAsync(conversation);

        Console.WriteLine(response.Message);
    }    
}