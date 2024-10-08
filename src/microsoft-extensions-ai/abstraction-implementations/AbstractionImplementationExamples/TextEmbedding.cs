using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task TextEmbedding() 
    {
        IEmbeddingGenerator<string,Embedding<float>> generator = 
            new SampleEmbeddingGenerator(new Uri("http://coolsite.ai"), "my-custom-model");

        var embeddings = await generator.GenerateAsync(new []{"What is AI?", "What is .NET?"});

        foreach(var embedding in embeddings)
        {
            Console.WriteLine(string.Join(", ", embedding.Vector.ToArray()));
        }
    }    
}