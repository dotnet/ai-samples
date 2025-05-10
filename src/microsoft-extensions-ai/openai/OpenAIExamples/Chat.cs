using OpenAI;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task Chat() 
    {
        IChatClient client =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .GetChatClient("gpt-4o-mini")
                .AsIChatClient();

        Console.WriteLine(await client.GetResponseAsync("What is AI?"));
    }    
}
