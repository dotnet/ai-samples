using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task Chat() 
    {
        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }    
}