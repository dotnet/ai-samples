using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

public class LoggingEmbeddingGenerator : DelegatingEmbeddingGenerator<string, Embedding<float>>
{
    private readonly ILogger _logger;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _innerGenerator;
    
    public LoggingEmbeddingGenerator(IEmbeddingGenerator<string, Embedding<float>> innerGenerator, ILogger? logger = null) : base(innerGenerator)
    {
        _innerGenerator = innerGenerator;
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LoggingEmbeddingGenerator>();
    }

    public override async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating embeddings for {count} values", values.Count());
        return await _innerGenerator.GenerateAsync(values, options, cancellationToken);
    }
}