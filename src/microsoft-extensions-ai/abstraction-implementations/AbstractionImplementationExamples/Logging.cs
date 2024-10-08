using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task LoggingChat() 
    {
        IChatClient sampleChatClient = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        IChatClient client = new LoggingChatClient(sampleChatClient);

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }    

    public static async Task LoggingEmbedding()
    {
        IEmbeddingGenerator<string,Embedding<float>> sampleEmbeddingGenerator = 
            new SampleEmbeddingGenerator(new Uri("http://coolsite.ai"), "my-custom-model");

        IEmbeddingGenerator<string,Embedding<float>> generator = 
            new LoggingEmbeddingGenerator(sampleEmbeddingGenerator);

        var embeddings = await generator.GenerateAsync(new []{"What is AI?", "What is .NET?"});

        foreach(var embedding in embeddings)
        {
            Console.WriteLine(string.Join(", ", embedding.Vector.ToArray()));
        }
    }
}