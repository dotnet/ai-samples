using System.ComponentModel;
using Microsoft.Extensions.AI;

public partial class OllamaSamples
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

        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "llama3.1";

        IChatClient client =
            new ChatClientBuilder()
                .UseFunctionInvocation()
                .Use(new OllamaChatClient(endpoint, modelId: modelId));

        var response = await client.CompleteAsync("Do I need an umbrella?", chatOptions);

        Console.WriteLine(response.Message);
    }    
}