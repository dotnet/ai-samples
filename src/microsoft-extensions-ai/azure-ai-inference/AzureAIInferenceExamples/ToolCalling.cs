using System.ComponentModel;
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;

public partial class AzureAIInferenceSamples
{
    public static async Task ToolCalling()
    {
        [Description("Gets the weather")]
        string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient client =
            new ChatCompletionsClient(endpoint, credential).AsChatClient(modelId)
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
