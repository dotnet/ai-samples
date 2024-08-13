#pragma warning disable
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

public class OllamaTextEmbeddingGenerationService : ITextEmbeddingGenerationService
{
    private readonly HttpClient _client; 
    private readonly string _modelId;
    public OllamaTextEmbeddingGenerationService(string modelId, Uri endpoint)
    {
        _client = new HttpClient();
        _client.BaseAddress = endpoint;
        _modelId = modelId;
    }

    public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text)
    {
        var response = await _client.PostAsJsonAsync("/api/embeddings", new {model = _modelId, prompt = text});
        var embedding = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
        return new ReadOnlyMemory<float>(embedding.Embedding);
    }

    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var embeddingResult = new List<ReadOnlyMemory<float>>();
        foreach(var item in data)
        {
            var response = await GenerateEmbeddingAsync(item);
            embeddingResult.Add(new ReadOnlyMemory<float>(response.ToArray()));
        }
        return embeddingResult;
    }

    class EmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; }
    }
}

public static class OllamaTextEmbeddingGenerationServiceExtensions
{
    public static IServiceCollection AddOllamaTextEmbeddingGenerationService(this IServiceCollection services, string modelId, Uri endpoint)
    {
        services.AddSingleton<ITextEmbeddingGenerationService>(_ => 
            { return new OllamaTextEmbeddingGenerationService(modelId, endpoint);});
        return services;
    }

    public static IKernelBuilder AddOllamaTextEmbeddingGeneration(this IKernelBuilder builder, string modelId, Uri endpoint)
    {
        builder.Services.AddOllamaTextEmbeddingGenerationService(modelId, endpoint);
        return builder;
    }
}