#pragma warning disable
using System.Numerics.Tensors;
using Microsoft.SemanticKernel.Embeddings;

public class ProductManualSemanticSearch
{
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private readonly IEnumerable<ManualChunk> _data;

    public ProductManualSemanticSearch(ITextEmbeddingGenerationService embeddingService, IEnumerable<ManualChunk> data)
    {
        _embeddingService = embeddingService;
        _data = data;
    }

    public async Task<IEnumerable<ManualChunk>> SearchAsync(int productId, string query, int limit=5)
    {
        Console.WriteLine($"Searching for '{query}' in product {productId}...");

        // [1] Embed query
        var queryEmbedding = await _embeddingService.GenerateEmbeddingsAsync(new[] {query});

        // [2] Embed data
        var dataEmbeddings = _data
            .Where(d => d.ProductId == productId)
            .Select(d => d.Embedding);

        // [3] Compute similarity
        var similarities = dataEmbeddings
            .Select(e => TensorPrimitives.CosineSimilarity(queryEmbedding.First().Span,e.AsSpan()))
            .ToList();

        // [4] Sort by similarity
        var results = similarities
            .Select((s, i) => new {Index = i, Similarity = s})
            .OrderByDescending(r => r.Similarity)
            .Take(5);

        // [5] Return top results
        return results.Select(r => _data.ElementAt(r.Index)).Take(limit);
    }
}