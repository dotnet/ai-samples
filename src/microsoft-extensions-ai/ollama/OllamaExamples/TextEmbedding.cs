using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task TextEmbedding() 
    {
        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "all-minilm";

        IEmbeddingGenerator<string,Embedding<float>> generator = new OllamaEmbeddingGenerator(endpoint, modelId: modelId);

        var embeddings = await generator.GenerateAsync("What is AI?");

        Console.WriteLine(string.Join(", ", embeddings[0].Vector.ToArray()));
    }    
}