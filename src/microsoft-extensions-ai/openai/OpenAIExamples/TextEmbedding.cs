using OpenAI;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task TextEmbedding() 
    {
        IEmbeddingGenerator<string,Embedding<float>> generator =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsEmbeddingGenerator("text-embedding-3-small");

        var embeddings = await generator.GenerateAsync("What is AI?");

        Console.WriteLine(string.Join(", ", embeddings[0].Vector.ToArray()));
    }    
}