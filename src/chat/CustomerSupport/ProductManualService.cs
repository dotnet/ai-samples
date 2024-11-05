public class ProductManualService
{
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly IVectorStore _store;
    private readonly IVectorStoreRecordCollection<int, ManualChunk> _collection;
    private readonly string _collectionName = "ProductManuals";

    public ProductManualService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IVectorStore store, IChatClient client)
    {
        _chatClient = client;
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

        var filter =
            new VectorSearchFilter()
                .EqualTo(nameof(ManualChunk.ProductId), productId);

        var searchOptions = new VectorSearchOptions
        {
            Top = limit ?? 1,
            Filter = filter,
            IncludeVectors = true
        };

        return await _collection.VectorizedSearchAsync(queryEmbedding, searchOptions);
    }
}
