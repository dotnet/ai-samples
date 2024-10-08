using OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class OpenAISamples
{
    public static async Task DependencyInjection() 
    {
        var app = Host.CreateApplicationBuilder();

        app.Services.AddSingleton(
            new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));
        app.Services.AddDistributedMemoryCache();
        app.Services.AddChatClient(builder => {

            var openaiClient = builder.Services.GetRequiredService<OpenAIClient>();
            var cache = builder.Services.GetRequiredService<IDistributedCache>();

            return builder
                .UseDistributedCache() 
                .Use(openaiClient.AsChatClient("gpt-4o-mini"));
        });

        var serviceProvider = app.Services.BuildServiceProvider();
        app.Build();

        var chatClient = serviceProvider.GetRequiredService<IChatClient>();

        var response = await chatClient.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }        
}