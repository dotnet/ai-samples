using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public partial class AbstractionSamples
{
    public static async Task Caching()
    {
        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model")
            .AsBuilder()
            .UseDistributedCache(cache)
            .Build();

        string[] prompts = ["What is AI?", "What is .NET?", "What is AI?"];

        foreach (var prompt in prompts)
        {
            await foreach (var message in client.CompleteStreamingAsync(prompt))
            {
                Console.WriteLine(message);
            }
        }
    }
}
