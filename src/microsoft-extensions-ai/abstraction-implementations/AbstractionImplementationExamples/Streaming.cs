using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task Streaming() 
    {
        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        var stream = client.CompleteStreamingAsync("What is AI?");

        await foreach(var update in stream)
        {
            Console.WriteLine(update);
        }
    }
}