using OpenAI;
using System.Diagnostics;
using Microsoft.Extensions.AI;
using OpenTelemetry.Trace;

public partial class OpenAISamples
{
    public static async Task OpenTelemetryExample() 
    {
        // Configure OpenTelemetry Exporter
        var sourceName = Guid.NewGuid().ToString();
        var activities = new List<Activity>();

        var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
            .AddSource(sourceName)
            .AddInMemoryExporter(activities)
            .Build();

        IChatClient openaiClient =
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                .AsChatClient("gpt-4o-mini");

        IChatClient client = openaiClient
            .AsBuilder()
            .UseOpenTelemetry(sourceName: sourceName, configure: o => o.EnableSensitiveData = true)
            .Build();

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }    
}
