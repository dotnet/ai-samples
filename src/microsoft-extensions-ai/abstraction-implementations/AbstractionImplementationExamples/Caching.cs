using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;

public partial class AbstractionSamples
{
    public static async Task Caching() 
    {
        IDistributedCache cache = new InMemoryCacheStorage();

        IChatClient client = 
            new ChatClientBuilder()
                .UseDistributedCache(cache)
                .Use(new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model"));

        var prompts = new []{"What is AI?", "What is .NET?", "What is AI?"};

        foreach(var prompt in prompts)
        {
            var response = client.CompleteStreamingAsync(prompt);

            await foreach(var message in response)
            {
                Console.WriteLine(message);
            }
        }
    }    
}