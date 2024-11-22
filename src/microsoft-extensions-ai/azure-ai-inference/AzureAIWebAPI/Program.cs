using Azure;
using Azure.AI.Inference;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.local.json",
    optional: true, 
    reloadOnChange: true);

builder.Services.AddSingleton(
    new ChatCompletionsClient(
        new Uri(builder.Configuration["AI:AzureAIInference:Chat:Endpoint"]),
        new AzureKeyCredential(builder.Configuration["AI:AzureAIInference:Key"])));

builder.Services.AddChatClient(services => services.GetRequiredService<ChatCompletionsClient>()
    .AsChatClient(builder.Configuration["AI:AzureAIInference:Chat:ModelId"]));

var app = builder.Build();

app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
    await client.CompleteAsync(message, cancellationToken: default));

app.Run();
