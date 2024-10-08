using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public partial class OllamaSamples
{
    public static async Task TextEmbeddingCaching() 
    {
        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "all-minilm";

        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        IEmbeddingGenerator<string,Embedding<float>> generator =
            new EmbeddingGeneratorBuilder<string, Embedding<float>>()
                .UseDistributedCache(cache)
                .Use(new OllamaEmbeddingGenerator(endpoint, modelId: modelId));

        var prompts = new []{"What is AI?", "What is .NET?", "What is AI?"};

        foreach(var prompt in prompts)
        {
            var embeddings = await generator.GenerateAsync("What is AI?");

            Console.WriteLine(string.Join(", ", embeddings[0].Vector.ToArray()));
        }
    }    
}