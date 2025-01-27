using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public partial class OllamaSamples
{
    public static async Task TextEmbeddingCaching() 
    {
        var endpoint = "http://localhost:11434/";
        var modelId = "all-minilm";

        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        IEmbeddingGenerator<string, Embedding<float>> generator = new OllamaEmbeddingGenerator(endpoint, modelId: modelId)
            .AsBuilder()
            .UseDistributedCache(cache)
            .Build();

        string[] prompts = ["What is AI?", "What is .NET?", "What is AI?"];

        foreach(var prompt in prompts)
        {
            var embedding = await generator.GenerateEmbeddingVectorAsync(prompt);

            Console.WriteLine(string.Join(", ", embedding.ToArray()));
        }
    }    
}
