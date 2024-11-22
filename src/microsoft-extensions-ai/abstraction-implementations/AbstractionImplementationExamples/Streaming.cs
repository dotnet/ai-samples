using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task Streaming()
    {
        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        await foreach (var update in client.CompleteStreamingAsync("What is AI?"))
        {
            Console.Write(update);
        }
        Console.WriteLine();
    }
}
