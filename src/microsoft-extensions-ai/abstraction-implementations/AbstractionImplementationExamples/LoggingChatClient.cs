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

    public override async Task<ChatResponse> GetResponseAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing chat for {message}", chatMessages.Last().Text);
        return await _innerClient.GetResponseAsync(chatMessages, options, cancellationToken);
    }

    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing chat for {message}", chatMessages.Last().Text);
        await foreach (var message in _innerClient.GetStreamingResponseAsync(chatMessages, options, cancellationToken))
        {
            yield return message;
        }
    }
}
