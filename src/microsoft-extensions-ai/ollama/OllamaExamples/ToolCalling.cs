using System.ComponentModel;
using Microsoft.Extensions.AI;

public partial class OllamaSamples
{
    public static async Task ToolCalling()
    {
        [Description("Gets the weather")]
        string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        var endpoint = "http://localhost:11434/";
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(endpoint, modelId: modelId)
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        Console.WriteLine(await client.CompleteAsync("Do I need an umbrella?", chatOptions));
    }
}
