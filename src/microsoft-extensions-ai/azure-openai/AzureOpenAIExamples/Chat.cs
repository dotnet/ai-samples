using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Azure.Identity;

public partial class OpenAISamples
{
    public static async Task Chat() 
    {
        IChatClient client =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                .GetChatClient("gpt-4o-mini")
                .AsIChatClient();

        Console.WriteLine(await client.GetResponseAsync("What is AI?"));
    }    
}
