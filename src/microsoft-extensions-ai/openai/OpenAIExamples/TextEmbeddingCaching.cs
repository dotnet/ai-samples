using OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;

public partial class OpenAISamples
{
    public static async Task TextEmbeddingCaching() 
    {
        IDistributedCache cache = new InMemoryCacheStorage();

        IEmbeddingGenerator<string,Embedding<float>> openAIGenerator =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsEmbeddingGenerator("text-embedding-3-small");

        IEmbeddingGenerator<string,Embedding<float>> generator =
            new EmbeddingGeneratorBuilder<string, Embedding<float>>()
                .UseDistributedCache(cache)
                .Use(openAIGenerator);

        var prompts = new [] {"What is AI?", "What is .NET?", "What is AI?"};

        foreach(var prompt in prompts)
        {
            var embeddings = await generator.GenerateAsync(prompt);

            Console.WriteLine(string.Join(", ", embeddings[0].Vector.ToArray()));
        }
    }    
}