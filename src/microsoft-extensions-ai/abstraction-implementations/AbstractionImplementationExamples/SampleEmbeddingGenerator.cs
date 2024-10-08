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
        var embeddings = new List<Embedding<float>>();

        foreach (var value in values)
        {
            // Simulate some async operation
            await Task.Delay(100, cancellationToken);

            // Generate a sample embedding
            var embedding = new Embedding<float>(new float[] {
                _random.NextSingle(),
                _random.NextSingle(),
                _random.NextSingle()});

            embeddings.Add(embedding);
        }

        return new GeneratedEmbeddings<Embedding<float>>(embeddings);
    }

    public TService? GetService<TService>(object? key = null) where TService : class
    {
        // Return null as this is a sample implementation
        return null;
    }

    public void Dispose()
    {
        // Clean up resources if necessary
    }
}