using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;

public partial class AzureAIInferenceSamples
{
    public static async Task Chat() 
    {
        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient client =
            new ChatCompletionsClient(endpoint, credential)
                .AsChatClient(modelId);

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }    
}
