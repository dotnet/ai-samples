using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class AbstractionSamples
{
    public static async Task DependencyInjection() 
    {
        var app = Host.CreateApplicationBuilder();

        app.Services.AddSingleton<IDistributedCache, InMemoryCacheStorage>();
        app.Services.AddChatClient(builder => {
            var endpoint = new Uri("http://coolsite.ai");
            var modelId = "my-custom-model";

            var cache = builder.Services.GetRequiredService<IDistributedCache>();

            return builder
                .UseDistributedCache(cache) 
                .Use(new SampleChatClient(endpoint, modelId));
        });

        var serviceProvider = app.Services.BuildServiceProvider();
        
        app.Build();

        var chatClient = serviceProvider.GetRequiredService<IChatClient>();

        var response = await chatClient.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }        
}