using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class AzureAIInferenceSamples
{
    public static async Task DependencyInjection()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddSingleton(
            new ChatCompletionsClient(
                new Uri("https://models.inference.ai.azure.com"),
                new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"))));

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddChatClient(services => services.GetRequiredService<ChatCompletionsClient>()
                .AsChatClient("gpt-4o-mini"))
            .UseDistributedCache()
            .Build();

        var app = builder.Build();

        var chatClient = app.Services.GetRequiredService<IChatClient>();

        Console.WriteLine(await chatClient.CompleteAsync("What is AI?"));
    }
}
