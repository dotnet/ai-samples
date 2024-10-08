using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task Chat() 
    {
        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId);

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }    
}