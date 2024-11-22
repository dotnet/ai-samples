using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

public class LoggingChatClient : DelegatingChatClient
{
    private readonly ILogger _logger;
    private readonly IChatClient _innerClient;

    public LoggingChatClient(IChatClient innerClient, ILogger? logger = null) : base(innerClient)
    {
        _innerClient = innerClient;
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LoggingChatClient>();
    }

    public override async Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing chat for {message}", chatMessages.Last().Text);
        return await _innerClient.CompleteAsync(chatMessages, options, cancellationToken);
    }

    public override async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing chat for {message}", chatMessages.Last().Text);
        await foreach (var message in _innerClient.CompleteStreamingAsync(chatMessages, options, cancellationToken))
        {
            yield return message;
        }
    }
}
