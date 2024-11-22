using System.Diagnostics;
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using OpenTelemetry.Trace;

public partial class AzureAIInferenceSamples
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

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var modelId = "gpt-4o-mini";
        var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"));

        IChatClient aiInferenceClient =
            new ChatCompletionsClient(endpoint, credential)
                .AsChatClient(modelId);

        IChatClient client = aiInferenceClient
            .AsBuilder()
            .UseOpenTelemetry(sourceName: sourceName, configure: o => o.EnableSensitiveData = true)
            .Build();

        Console.WriteLine(await client.CompleteAsync("What is AI?"));
    }
}
