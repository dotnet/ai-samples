using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task Streaming()
    {
        var endpoint = "http://localhost:11434/";
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId);

        await foreach (var update in client.CompleteStreamingAsync("What is AI?"))
        {
            Console.Write(update);
        }
        Console.WriteLine();
    }
}
