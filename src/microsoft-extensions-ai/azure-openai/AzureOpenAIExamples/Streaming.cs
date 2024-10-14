using OpenAI;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;

public partial class OpenAISamples
{
    public static async Task Streaming() 
    {
        IChatClient client =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                .AsChatClient(modelId: "gpt-4o-mini");

        var stream = client.CompleteStreamingAsync("What is AI?");
        await foreach (var update in stream)
        {
            Console.Write(update);
        }
    }    
}