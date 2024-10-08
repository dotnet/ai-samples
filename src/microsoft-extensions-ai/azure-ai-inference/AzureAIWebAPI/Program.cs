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

builder.Services.AddChatClient(c => {
    var azureAIInferenceClient = c.Services.GetRequiredService<ChatCompletionsClient>();
    var modelId = builder.Configuration["AI:AzureAIInference:Chat:ModelId"];
    return c.Use(azureAIInferenceClient.AsChatClient(modelId));
});

var app = builder.Build();

app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
{
    var response = await client.CompleteAsync(message, cancellationToken: default);
    return response;
});

app.Run();