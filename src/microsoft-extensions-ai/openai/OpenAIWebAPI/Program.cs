using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddSingleton(new OpenAIClient(builder.Configuration["AI:OpenAI:Key"]));

builder.Services.AddChatClient(c => {
    var openAIClient = c.Services.GetRequiredService<OpenAIClient>();
    var modelId = builder.Configuration["AI:OpenAI:Chat:ModelId"] ?? "gpt-4o-mini";
    return c.Use(openAIClient.AsChatClient(modelId));
});

builder.Services.AddEmbeddingGenerator<string,Embedding<float>>(c => {
    var openAIClient = c.Services.GetRequiredService<OpenAIClient>();
    var modelId = builder.Configuration["AI:OpenAI:Embedding:ModelId"] ?? "text-embedding-3-small";
    return c.Use(openAIClient.AsEmbeddingGenerator(modelId));
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