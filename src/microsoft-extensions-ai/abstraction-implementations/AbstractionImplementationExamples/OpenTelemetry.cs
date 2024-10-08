using System.Diagnostics;
using Microsoft.Extensions.AI;
using OpenTelemetry.Trace;

public partial class AbstractionSamples
{
    public static async Task OpenTelemetryExample() 
    {
        // Configure OpenTelemetry Exporter
        var sourceName  = Guid.NewGuid().ToString();
        var activities = new List<Activity>();

        var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
            .AddSource(sourceName)
            .AddInMemoryExporter(activities)
            .Build();

        IChatClient client =
            new ChatClientBuilder()
                .UseOpenTelemetry(sourceName, instance => {
                    instance.EnableSensitiveData = true;
                })
                .Use(new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model"));

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }    
}