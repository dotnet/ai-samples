using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task LoggingChat()
    {
        IChatClient sampleChatClient = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");

        IChatClient client = new LoggingChatClient(sampleChatClient);

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }

    public static async Task LoggingEmbedding()
    {
        IEmbeddingGenerator<string, Embedding<float>> generator =
            new LoggingEmbeddingGenerator(
                new SampleEmbeddingGenerator(new Uri("http://coolsite.ai"), "my-custom-model"));

        foreach (var embedding in await generator.GenerateAsync(["What is AI?", "What is .NET?"]))
        {
            Console.WriteLine(string.Join(", ", embedding.Vector.ToArray()));
        }
    }
}
