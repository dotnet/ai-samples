using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddChatClient(c => {
    var endpoint = new Uri(builder.Configuration["AI:Ollama:Chat:Endpoint"] ?? "http://localhost:11434/");
    var modelId = builder.Configuration["AI:Ollama:Chat:ModelId"];
    return c.Use(new OllamaChatClient(endpoint, modelId));
});

builder.Services.AddEmbeddingGenerator<string,Embedding<float>>(c => {
    var endpoint = new Uri(builder.Configuration["AI:Ollama:Embedding:Endpoint"] ?? "http://localhost:11434/");
    var modelId = builder.Configuration["AI:Ollama:Embedding:ModelId"];
    
    return c.Use(new OllamaEmbeddingGenerator(endpoint, modelId));
});

var app = builder.Build();

app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
{
    var response = await client.CompleteAsync(message, cancellationToken: default);
    return response;
});

app.MapPost("/embedding", async (IEmbeddingGenerator<string,Embedding<float>> client, [FromBody] string message) =>
{
    var response = await client.GenerateAsync(message);
    return response;
});

app.Run();