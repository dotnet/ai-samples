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

        var endpoint = new Uri("http://localhost:11434/");
        var modelId = "llama3.1";

        IChatClient client =
            new ChatClientBuilder()
                .UseOpenTelemetry(sourceName, instance => {
                    instance.EnableSensitiveData = true;
                })
                .Use(new OllamaChatClient(endpoint, modelId: modelId));

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }    
}