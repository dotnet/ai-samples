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
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient aiInferenceClient =
            new ChatCompletionsClient(endpoint, credential)
                .AsChatClient(modelId);

        IChatClient client =
            new ChatClientBuilder()
                .UseDistributedCache(cache)
                .Use(aiInferenceClient);

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