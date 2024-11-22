using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public partial class AbstractionSamples
{
    public static async Task TextEmbeddingCaching()
    {
        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        IEmbeddingGenerator<string, Embedding<float>> sampleEmbeddingGenerator =
            new SampleEmbeddingGenerator(new Uri("http://coolsite.ai"), "my-custom-model");

        IEmbeddingGenerator<string, Embedding<float>> generator =
            sampleEmbeddingGenerator
            .AsBuilder()
            .UseDistributedCache(cache)
            .Build();

        var embeddings = await generator.GenerateAsync([
            "What is AI?",
            "What is .NET?",
            "What is AI?"]);

        foreach (var embedding in embeddings)
        {
            Console.WriteLine(string.Join(", ", embedding.Vector.ToArray()));
        }
    }
}
