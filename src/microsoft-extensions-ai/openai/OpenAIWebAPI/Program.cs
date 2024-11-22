using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddSingleton(new OpenAIClient(builder.Configuration["AI:OpenAI:Key"]));

builder.Services.AddChatClient(services =>
    services.GetRequiredService<OpenAIClient>().AsChatClient(builder.Configuration["AI:OpenAI:Chat:ModelId"] ?? "gpt-4o-mini"));

builder.Services.AddEmbeddingGenerator(services =>
    services.GetRequiredService<OpenAIClient>().AsEmbeddingGenerator(builder.Configuration["AI:OpenAI:Embedding:ModelId"] ?? "text-embedding-3-small"));

var app = builder.Build();

app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
    await client.CompleteAsync(message, cancellationToken: default));

app.MapPost("/embedding", async (IEmbeddingGenerator<string, Embedding<float>> client, [FromBody] string message) =>
    await client.GenerateEmbeddingAsync(message));

app.Run();
