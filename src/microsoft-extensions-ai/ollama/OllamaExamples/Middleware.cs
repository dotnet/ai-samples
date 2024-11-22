using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

public partial class OllamaSamples
{
    public static async Task Middleware() 
    {
        // Configure OpenTelemetry Exporter
        var sourceName  = Guid.NewGuid().ToString();
        var activities = new List<Activity>();

        var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
            .AddSource(sourceName)
            .AddInMemoryExporter(activities)
            // .AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"))
            .Build();

        // Configure Cache
        var options = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(options);

        // Configure tool calling
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
