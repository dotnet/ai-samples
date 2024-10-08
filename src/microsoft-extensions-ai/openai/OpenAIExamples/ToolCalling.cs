using OpenAI;
using System.ComponentModel;
using Microsoft.Extensions.AI;

public partial class OpenAISamples
{
    public static async Task ToolCalling() 
    {
        [Description("Gets the weather")]
        string GetWeather()
        {
            var r = new Random();
            return r.NextDouble() > 0.5 ? "It's sunny" : "It's raining";
        }

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        IChatClient openaiClient =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsChatClient("gpt-4o-mini");

        IChatClient client =
            new ChatClientBuilder()
                .UseFunctionInvocation()
                .Use(openaiClient);

        var stream = client.CompleteStreamingAsync("Do I need an umbrella?", chatOptions);

        await foreach (var message in stream)
        {
            Console.Write(message);
        }
    }    
}