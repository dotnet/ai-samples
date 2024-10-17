using OpenAI;
using System.Diagnostics;
using Microsoft.Extensions.AI;
using OpenTelemetry.Trace;
using Azure.AI.OpenAI;
using Azure.Identity;

public partial class OpenAISamples
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

        IChatClient azureOpenAIClient =
            new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
                new DefaultAzureCredential())
                    .AsChatClient("gpt-4o-mini");

        IChatClient client =
            new ChatClientBuilder()
                .UseOpenTelemetry(sourceName, instance => {
                    instance.EnableSensitiveData = true;
                })
                .Use(azureOpenAIClient);

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }    
}