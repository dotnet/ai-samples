using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task Streaming() 
    {
        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId);

        var stream = client.CompleteStreamingAsync("What is AI?");
        await foreach (var update in stream)
        {
            Console.Write(update);
        }
    }    
}