using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;

public partial class OpenAISamples
{
    public static async Task TextEmbedding() 
    {
        IEmbeddingGenerator<string,Embedding<float>> generator =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                    .AsEmbeddingGenerator("text-embedding-3-small");

        var embedding = await generator.GenerateEmbeddingVectorAsync("What is AI?");

        Console.WriteLine(string.Join(", ", embedding.ToArray()));
    }    
}
