using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class OllamaSamples
{
    public static async Task DependencyInjection() 
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddChatClient(new OllamaChatClient("http://localhost:11434/", "llama3.1"))
            .UseDistributedCache();

        var app = builder.Build();

        var chatClient = app.Services.GetRequiredService<IChatClient>();

        Console.WriteLine(await chatClient.CompleteAsync("What is AI?"));
    }    
}
