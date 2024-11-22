using Microsoft.Extensions.AI;

public class SampleEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    private readonly Random _random = new Random();
    private readonly Uri _serviceEndpoint;
    private readonly string _modelId;

    public EmbeddingGeneratorMetadata Metadata { get; }

    public SampleEmbeddingGenerator(Uri endpoint, string modelId)
    {
        _serviceEndpoint = endpoint;
        _modelId = modelId;
        Metadata = new EmbeddingGeneratorMetadata("SampleEmbeddingGenerator", endpoint, modelId);
    }

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        GeneratedEmbeddings<Embedding<float>> embeddings = [];

        foreach (var value in values)
        {
            // Simulate some async operation
            await Task.Delay(100, cancellationToken);

            // Generate a sample embedding
            embeddings.Add(new(new[] {
                _random.NextSingle(),
                _random.NextSingle(),
                _random.NextSingle()}));
        }

        return embeddings;
    }

    public object? GetService(Type serviceType, object? key = null) =>
        key is null && serviceType?.IsInstanceOfType(this) is true ? this : null;

    public void Dispose()
    {
        // Clean up resources if necessary
    }
}
