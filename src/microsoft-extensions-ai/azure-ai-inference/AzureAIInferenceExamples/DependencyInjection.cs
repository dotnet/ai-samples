using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class AzureAIInferenceSamples
{
    public static async Task DependencyInjection() 
    {
        var app = Host.CreateApplicationBuilder();

        app.Services.AddSingleton(
            new ChatCompletionsClient(
                new Uri("https://models.inference.ai.azure.com"),
                new AzureKeyCredential(Environment.GetEnvironmentVariable("GH_TOKEN"))));

        app.Services.AddDistributedMemoryCache();
        app.Services.AddChatClient(builder => {

            IChatClient aiInferenceClient = 
                builder.Services.GetRequiredService<ChatCompletionsClient>()
                    .AsChatClient("gpt-4o-mini");
            
            return builder
                .UseDistributedCache() 
                .Use(aiInferenceClient);
        });

        var serviceProvider = app.Services.BuildServiceProvider();
        
        app.Build();

        var chatClient = serviceProvider.GetRequiredService<IChatClient>();

        var response = await chatClient.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }        
}