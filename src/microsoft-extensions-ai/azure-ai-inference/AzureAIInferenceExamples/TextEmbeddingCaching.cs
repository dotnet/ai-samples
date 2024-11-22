using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Azure.AI.Inference;
using Azure;

public partial class AzureAIInferenceSamples
{
    public static async Task TextEmbeddingCaching() 
    {
        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "text-embedding-3-small";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IEmbeddingGenerator<string, Embedding<float>> generator =
            new EmbeddingsClient(endpoint, credential).AsEmbeddingGenerator(modelId)
            .AsBuilder()
            .UseDistributedCache(cache)
            .Build();

        string[] prompts = ["What is AI?", "What is .NET?", "What is AI?"];

        foreach (var prompt in prompts)
        {
            var embedding = await generator.GenerateEmbeddingVectorAsync(prompt);

            Console.WriteLine(string.Join(", ", embedding.ToArray()));
        }
    }    
}
