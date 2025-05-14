using OpenAI;
using System.ComponentModel;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task ToolCalling()
    {
        [Description("Gets the weather")]
        string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        IChatClient openaiClient =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .GetChatClient("gpt-4o-mini")
                .AsIChatClient();

        IChatClient client = openaiClient
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        await foreach (var message in client.GetStreamingResponseAsync("Do I need an umbrella?", chatOptions))
        {
            Console.Write(message);
        }
        Console.WriteLine();
    }
}
