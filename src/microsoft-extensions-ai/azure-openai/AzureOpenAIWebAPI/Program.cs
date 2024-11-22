using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddSingleton(
    new AzureOpenAIClient(
        new Uri(builder.Configuration["AI:AzureOpenAI:Endpoint"]),
        new DefaultAzureCredential()
    ));

builder.Services.AddChatClient(services => services.GetRequiredService<AzureOpenAIClient>()
    .AsChatClient(builder.Configuration["AI:AzureOpenAI:Chat:ModelId"] ?? "gpt-4o-mini"));

builder.Services.AddEmbeddingGenerator(services => services.GetRequiredService<AzureOpenAIClient>()
    .AsEmbeddingGenerator(builder.Configuration["AI:AzureOpenAI:Embedding:ModelId"] ?? "text-embedding-3-small"));

var app = builder.Build();

app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
    await client.CompleteAsync(message, cancellationToken: default));

app.MapPost("/embedding", async (IEmbeddingGenerator<string, Embedding<float>> client, [FromBody] string message) =>
    await client.GenerateEmbeddingAsync(message));

app.Run();
