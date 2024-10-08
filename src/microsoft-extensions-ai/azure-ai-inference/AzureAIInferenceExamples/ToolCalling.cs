using System.ComponentModel;
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;

public partial class AzureAIInferenceSamples
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

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient aiInferenceClient =
            new ChatCompletionsClient(endpoint,credential)
                .AsChatClient(modelId);

        IChatClient client =
            new ChatClientBuilder()
                .UseFunctionInvocation()
                .Use(aiInferenceClient);

        var stream = client.CompleteStreamingAsync("Do I need an umbrella?", chatOptions);

        await foreach (var message in stream)
        {
            Console.Write(message);
        }
    }    
}