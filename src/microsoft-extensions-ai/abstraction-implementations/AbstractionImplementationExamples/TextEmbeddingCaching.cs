using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;

public partial class AbstractionSamples
{
    public static async Task TextEmbeddingCaching() 
    {
        // Configure cache
        IDistributedCache cache = new InMemoryCacheStorage();

        IEmbeddingGenerator<string,Embedding<float>> sampleEmbeddingGenerator = 
            new SampleEmbeddingGenerator(new Uri("http://coolsite.ai"), "my-custom-model");

        IEmbeddingGenerator<string,Embedding<float>> generator =
            new EmbeddingGeneratorBuilder<string, Embedding<float>>()
                .UseDistributedCache(cache)
                .Use(sampleEmbeddingGenerator);

        var embeddings = await generator.GenerateAsync(new []{
            "What is AI?",
            "What is .NET?",
            "What is AI?"});

        foreach(var embedding in embeddings)
        {
            Console.WriteLine(string.Join(", ", embedding.Vector.ToArray()));
        }
    }    
}