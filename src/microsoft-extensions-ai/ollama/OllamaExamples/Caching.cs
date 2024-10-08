using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public partial class OllamaSamples
{
    public static async Task Caching() 
    {
        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "llama3.1";

        IChatClient client =
            new ChatClientBuilder()
                .UseDistributedCache(cache)
                .Use(new OllamaChatClient(endpoint, modelId: modelId));

        var prompts = new []{"What is AI?", "What is .NET?", "What is AI?"};

        foreach(var prompt in prompts)
        {
            var stream = client.CompleteStreamingAsync(prompt);
            await foreach (var message in stream)
            {
                Console.Write(message);
            }
        }
    }    
}