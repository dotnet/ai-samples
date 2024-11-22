using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task ConversationHistory()
    {
        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        List<ChatMessage> conversation =
        [
            new(ChatRole.System, "You are a helpful AI assistant"),
            new(ChatRole.User, "What is AI?")
        ];

        Console.WriteLine(await client.CompleteAsync(conversation));
    }
}
