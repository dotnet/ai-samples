using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.local.json",
    optional: true,
    reloadOnChange: true);

var chatEndpoint = new Uri(builder.Configuration["AI:Ollama:Chat:Endpoint"] ?? "http://localhost:11434/");
var embeddingEndpoint = new Uri(builder.Configuration["AI:Ollama:Embedding:Endpoint"] ?? "http://localhost:11434/");
var chatModelId = builder.Configuration["AI:Ollama:Chat:ModelId"];
var embeddingModelId = builder.Configuration["AI:Ollama:Embedding:ModelId"];

builder.Services.AddChatClient(new OllamaChatClient(chatEndpoint, chatModelId));
builder.Services.AddEmbeddingGenerator(new OllamaEmbeddingGenerator(embeddingEndpoint, embeddingModelId));

var app = builder.Build();

app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
    await client.CompleteAsync(message, cancellationToken: default));

app.MapPost("/embedding", async (IEmbeddingGenerator<string, Embedding<float>> client, [FromBody] string message) =>
    await client.GenerateEmbeddingAsync(message));

app.Run();
