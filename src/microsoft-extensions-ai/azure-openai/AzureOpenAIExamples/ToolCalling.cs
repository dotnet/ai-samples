using OpenAI;
using System.ComponentModel;
using Microsoft.Extensions.AI;
using Azure.Identity;
using Azure.AI.OpenAI;

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

        IChatClient azureOpenAIClient =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                    .AsChatClient("gpt-4o-mini");

        IChatClient client =
            new ChatClientBuilder()
                .UseFunctionInvocation()
                .Use(azureOpenAIClient);

        var stream = client.CompleteStreamingAsync("Do I need an umbrella?", chatOptions);

        await foreach (var message in stream)
        {
            Console.Write(message);
        }
    }    
}