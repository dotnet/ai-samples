using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task Chat() 
    {
        var endpoint = "http://localhost:11434/";
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId);

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }    
}
