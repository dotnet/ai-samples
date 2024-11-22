using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task TextEmbedding() 
    {
        var endpoint = "http://localhost:11434/";
        var modelId = "all-minilm";

        IEmbeddingGenerator<string,Embedding<float>> generator = new OllamaEmbeddingGenerator(endpoint, modelId: modelId);

        var embedding = await generator.GenerateEmbeddingVectorAsync("What is AI?");

        Console.WriteLine(string.Join(", ", embedding.ToArray()));
    }    
}
