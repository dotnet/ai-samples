using Microsoft.Extensions.AI;
using Azure.AI.Inference;
using Azure;

public partial class AzureAIInferenceSamples
{
    public static async Task TextEmbedding() 
    {
        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "text-embedding-3-small";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IEmbeddingGenerator<string, Embedding<float>> generator =
            new EmbeddingsClient(endpoint, credential).AsEmbeddingGenerator(modelId);

        var embedding = await generator.GenerateEmbeddingVectorAsync("What is AI?");

        Console.WriteLine(string.Join(", ", embedding.ToArray()));
    }    
}
