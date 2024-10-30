using OpenAI;
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

        var embeddings = await generator.GenerateAsync("What is AI?");

        Console.WriteLine(string.Join(", ", embeddings[0].Vector.ToArray()));
    }    
}