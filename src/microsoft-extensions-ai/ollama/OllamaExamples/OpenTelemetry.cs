using System.Diagnostics;
using Microsoft.Extensions.AI;
using OpenTelemetry.Trace;

public partial class OllamaSamples
{
    public static async Task OpenTelemetryExample() 
    {
        // Configure OpenTelemetry Exporter
        var sourceName  = Guid.NewGuid().ToString();
        var activities = new List<Activity>();

        var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
            .AddSource(sourceName)
            .AddInMemoryExporter(activities) // Consider OtlpExporter for your application
            .Build();

        IChatClient client = new OllamaChatClient("http://localhost:11434/", "llama3.1")
            .AsBuilder()
            .UseOpenTelemetry(sourceName: sourceName, configure: o => o.EnableSensitiveData = true)
            .Build();

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }    
}
