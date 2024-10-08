using OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;

public partial class OpenAISamples
{
    public static async Task Caching() 
    {
        IDistributedCache cache = new InMemoryCacheStorage();

        IChatClient openaiClient =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsChatClient("gpt-4o-mini");

        IChatClient client =
            new ChatClientBuilder()
                .UseDistributedCache(cache)
                .Use(openaiClient);

        var prompts = new []{"What is AI?", "What is .NET?", "What is AI?"};

        foreach(var prompt in prompts)
        {
            var stream = client.CompleteStreamingAsync(prompt);
            await foreach (var message in stream)
            {
                Console.Write(message);
            }
        }
    }    
}