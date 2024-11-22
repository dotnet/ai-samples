using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task Chat()
    {
        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }
}
