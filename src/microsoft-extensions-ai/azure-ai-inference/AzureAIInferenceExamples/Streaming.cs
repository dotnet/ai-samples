using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;

public partial class AzureAIInferenceSamples
{
    public static async Task Streaming() 
    {
        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient client =
            new ChatCompletionsClient(endpoint, credential)
                .AsChatClient(modelId);

        var stream = client.CompleteStreamingAsync("What is AI?");
        
        await foreach (var update in stream)
        {
            Console.Write(update);
        }
    }    
}