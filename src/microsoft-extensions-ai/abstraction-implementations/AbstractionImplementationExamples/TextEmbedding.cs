using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task TextEmbedding()
    {
        IEmbeddingGenerator<string, Embedding<float>> generator =
            new SampleEmbeddingGenerator(new Uri("http://coolsite.ai"), "my-custom-model");

        foreach (var embedding in await generator.GenerateAsync(["What is AI?", "What is .NET?"]))
        {
            Console.WriteLine(string.Join(", ", embedding.Vector.ToArray()));
        }
    }
}
