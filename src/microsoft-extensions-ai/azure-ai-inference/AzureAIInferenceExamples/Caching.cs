using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public partial class AzureAIInferenceSamples
{
    public static async Task Caching()
    {
        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient client = new ChatCompletionsClient(endpoint, credential).AsChatClient("gpt-4o-mini")
            .AsBuilder()
            .UseDistributedCache(cache)
            .Build();

        string[] prompts = ["What is AI?", "What is .NET?", "What is AI?"];

        foreach (var prompt in prompts)
        {
            await foreach (var message in client.CompleteStreamingAsync(prompt))
            {
                Console.Write(message);
            }

            Console.WriteLine();
        }
    }
}
