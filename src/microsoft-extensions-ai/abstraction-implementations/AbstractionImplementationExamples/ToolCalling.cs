using System.ComponentModel;
using Microsoft.Extensions.AI;

public partial class AbstractionSamples
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
            Tools = [AIFunctionFactory.Create(GetWeather) ]
        };

        IChatClient client =
            new ChatClientBuilder()
                .UseFunctionInvocation()
                .Use(new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model"));

        var response = client.CompleteStreamingAsync("What is AI?", chatOptions);

        await foreach(var message in response)
        {
            Console.WriteLine(message);
        }
    }    
}