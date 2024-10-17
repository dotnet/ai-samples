using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OpenAI;

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

builder.Services.AddChatClient(c => {
    var openAIClient = c.Services.GetRequiredService<AzureOpenAIClient>();
    var modelId = builder.Configuration["AI:AzureOpenAI:Chat:ModelId"] ?? "gpt-4o-mini";
    return c.Use(openAIClient.AsChatClient(modelId));
});

builder.Services.AddEmbeddingGenerator<string,Embedding<float>>(c => {
    var openAIClient = c.Services.GetRequiredService<AzureOpenAIClient>();
    var modelId = builder.Configuration["AI:AzureOpenAI:Embedding:ModelId"] ?? "text-embedding-3-small";
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