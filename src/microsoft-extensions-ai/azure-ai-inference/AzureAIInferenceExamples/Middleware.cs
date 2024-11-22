using System.ComponentModel;
using System.Diagnostics;
using Azure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

public partial class AzureAIInferenceSamples
{
    public static async Task Middleware()
    {
        // Configure OpenTelemetry Exporter
        var sourceName = Guid.NewGuid().ToString();
        var activities = new List<Activity>();

        var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
            .AddSource(sourceName)
            .AddInMemoryExporter(activities)
            .Build();

        // Configure cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        // Configure tool calling
        [Description("Gets the weather")]
        string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        var aiInferenceClient =
            new Azure.AI.Inference.ChatCompletionsClient(endpoint, credential)
                .AsChatClient(modelId);

        var client = aiInferenceClient
            .AsBuilder()
            .UseFunctionInvocation()
            .UseOpenTelemetry(sourceName: sourceName, configure: o => o.EnableSensitiveData = true)
            .UseDistributedCache(cache)
            .Build();

        List<ChatMessage> conversation =
        [
            new(ChatRole.System, "You are a helpful AI assistant"),
            new(ChatRole.User, "Do I need an umbrella?")
        ];

        Console.WriteLine(await client.CompleteAsync("Do I need an umbrella?", chatOptions));
    }
}
