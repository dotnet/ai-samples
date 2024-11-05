public class ProductManualService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly IVectorStore _store;
    private readonly IVectorStoreRecordCollection<int, ManualChunk> _collection;
    private readonly string _collectionName = "ProductManuals";
    private readonly string _vectorField = "Embedding";
    public ProductManualService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IVectorStore store)
    {
        _embeddingGenerator = embeddingGenerator;
        _store = store;
        _collection = _store.GetCollection<int, ManualChunk>(_collectionName);
        Task.FromResult(_collection.CreateCollectionIfNotExistsAsync());
    }

    public async Task InsertManualChunksAsync(IEnumerable<ManualChunk> manualChunks)
    {
        foreach (var chunk in manualChunks)
        {
            await _collection.UpsertAsync(chunk);
        }
    }

    public async Task<VectorSearchResults<ManualChunk>> GetManualChunksAsync(string query, int? productId, int? limit = 5)
    {
        var queryEmbedding = await _embeddingGenerator.GenerateEmbeddingVectorAsync(query);

        var searchOptions = new VectorSearchOptions
        {
            Top = limit ?? 1,
            VectorPropertyName = _vectorField,
            // TODO: Filter not working correctly
            //Filter = new VectorSearchFilter(new List<FilterClause>()
            //{
            //    new EqualToFilterClause(nameof(ManualChunk.ProductId), productId)
            //})
        };

        return await _collection.VectorizedSearchAsync(queryEmbedding, searchOptions);
    }
}
