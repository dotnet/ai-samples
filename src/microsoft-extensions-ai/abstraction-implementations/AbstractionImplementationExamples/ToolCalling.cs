using System.ComponentModel;
using Microsoft.Extensions.AI;

public partial class AbstractionSamples
{
    public static async Task ToolCalling()
    {
        [Description("Gets the weather")]
        string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model")
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        await foreach (var message in client.CompleteStreamingAsync("What is AI?", chatOptions))
        {
            Console.Write(message);
        }
        Console.WriteLine();
    }
}
