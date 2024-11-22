using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class AbstractionSamples
{
    public static async Task DependencyInjection()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddChatClient(new SampleChatClient(new("http://coolsite.ai"), "my-custom-model"))
            .UseDistributedCache();

        var app = builder.Build();

        var chatClient = app.Services.GetRequiredService<IChatClient>();

        Console.WriteLine(await chatClient.CompleteAsync("What is AI?"));
    }
}
