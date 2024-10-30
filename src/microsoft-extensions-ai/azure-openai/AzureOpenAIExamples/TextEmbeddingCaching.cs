using OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Azure.AI.OpenAI;
using Azure.Identity;

public partial class OpenAISamples
{
    public static async Task TextEmbeddingCaching() 
    {
        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        IEmbeddingGenerator<string,Embedding<float>> azureOpenAIGenerator =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                    .AsEmbeddingGenerator("text-embedding-3-small");

        IEmbeddingGenerator<string,Embedding<float>> generator =
            new EmbeddingGeneratorBuilder<string, Embedding<float>>()
                .UseDistributedCache(cache)
                .Use(azureOpenAIGenerator);

        var prompts = new [] {"What is AI?", "What is .NET?", "What is AI?"};

        foreach(var prompt in prompts)
        {
            var embeddings = await generator.GenerateAsync(prompt);

            Console.WriteLine(string.Join(", ", embeddings[0].Vector.ToArray()));
        }
    }    
}