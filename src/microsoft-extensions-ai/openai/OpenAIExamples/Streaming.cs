using OpenAI;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task Streaming() 
    {
        IChatClient client =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .GetChatClient("gpt-4o-mini").AsIChatClient();

        await foreach (var update in client.GetStreamingResponseAsync("What is AI?"))
        {
            Console.Write(update);
        }
        Console.WriteLine();
    }    
}
