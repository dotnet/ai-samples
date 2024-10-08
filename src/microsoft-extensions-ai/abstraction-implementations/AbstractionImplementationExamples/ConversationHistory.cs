using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task ConversationHistory() 
    {
        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        var conversation = new [] {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "What is AI?")
        };        

        var response = await client.CompleteAsync(conversation);

        Console.WriteLine(response.Message);
    }
}