using OpenAI;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

public partial class OpenAISamples
{
    public static async Task Middleware() 
    {
        // Configure OpenTelemetry Exporter
        var sourceName  = Guid.NewGuid().ToString();
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
        string GetWeather()
        {
            var r = new Random();
            return r.NextDouble() > 0.5 ? "It's sunny" : "It's raining";
        }

        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)]
        };

        IChatClient openaiClient =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsChatClient("gpt-4o-mini");

        IChatClient client =
            new ChatClientBuilder()
                .UseFunctionInvocation()
                .UseOpenTelemetry(sourceName, instance => {
                    instance.EnableSensitiveData = true;
                })
                .UseDistributedCache(cache)
                .Use(openaiClient);

        var conversation = new [] {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "Do I need an umbrella?")
        };

        var response = await client.CompleteAsync("Do I need an umbrella?", chatOptions);

        Console.WriteLine(response.Message);
    }
}