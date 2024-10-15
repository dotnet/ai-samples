#pragma warning disable
using System.Numerics.Tensors;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Embeddings;

public class ProductManualSemanticSearch
{
    private readonly IEmbeddingGenerator<string,Embedding<float>> _embeddingGenerator;
    private readonly IEnumerable<ManualChunk> _data;

    public ProductManualSemanticSearch(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IEnumerable<ManualChunk> data)
    {
        _embeddingGenerator = embeddingGenerator;
        _data = data;
    }

    public async Task<IEnumerable<ManualChunk>> SearchAsync(int productId, string query, int limit=5)
    {
        Console.WriteLine($"Searching for '{query}' in product {productId}...");

        // [1] Embed query
        var queryEmbedding = await _embeddingGenerator.GenerateAsync(new[] {query});

        // [2] Embed data
        var dataEmbeddings = _data
            .Where(d => d.ProductId == productId)
            .Select(d => d.Embedding.Vector);

        // [3] Compute similarity
        var similarities = dataEmbeddings
            .Select(e => TensorPrimitives.CosineSimilarity(queryEmbedding.First().Vector.Span,e.Span))
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
