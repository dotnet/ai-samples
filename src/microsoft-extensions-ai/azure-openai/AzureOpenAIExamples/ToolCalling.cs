using System.ComponentModel;
using Microsoft.Extensions.AI;
using Azure.Identity;
using Azure.AI.OpenAI;

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

        IChatClient azureOpenAIClient =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                    .AsChatClient("gpt-4o-mini");

        IChatClient client = azureOpenAIClient
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        await foreach (var message in client.CompleteStreamingAsync("Do I need an umbrella?", chatOptions))
        {
            Console.Write(message);
        }
        Console.WriteLine();
    }    
}
