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

        IEmbeddingGenerator<string, Embedding<float>> azureOpenAIGenerator =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                    .AsEmbeddingGenerator("text-embedding-3-small");

        IEmbeddingGenerator<string, Embedding<float>> generator = azureOpenAIGenerator
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
