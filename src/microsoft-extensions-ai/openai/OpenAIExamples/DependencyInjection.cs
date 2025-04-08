﻿using OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class OpenAISamples
{
    public static async Task DependencyInjection() 
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddSingleton(new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddChatClient(services => services.GetRequiredService<OpenAIClient>().GetChatClient("gpt-4o-mini").AsIChatClient())
            .UseDistributedCache();
        
        var app = builder.Build();

        var chatClient = app.Services.GetRequiredService<IChatClient>();

        Console.WriteLine(await chatClient.GetResponseAsync("What is AI?"));
    }        
}
