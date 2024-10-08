using OpenAI;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task Streaming() 
    {
        IChatClient client =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsChatClient("gpt-4o-mini");

        var stream = client.CompleteStreamingAsync("What is AI?");
        await foreach (var update in stream)
        {
            Console.Write(update);
        }
    }    
}