using OpenAI;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task ConversationHistory() 
    {
        IChatClient client =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .GetChatClient("gpt-4o-mini")
                .AsIChatClient();

        List<ChatMessage> conversation =
        [
            new(ChatRole.System, "You are a helpful AI assistant"),
            new(ChatRole.User, "What is AI?")
        ];

        Console.WriteLine(await client.GetResponseAsync(conversation));
    }    
}
