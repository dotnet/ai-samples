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

        await foreach (var update in client.CompleteStreamingAsync("What is AI?"))
        {
            Console.Write(update);
        }
        Console.WriteLine();
    }    
}
