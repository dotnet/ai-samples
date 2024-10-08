using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

public class SampleChatClient : IChatClient
{
    private readonly Random _random = new Random();
    private readonly Uri _serviceEndpoint;
    private readonly string _modelId;

    public ChatClientMetadata Metadata { get; }

    public SampleChatClient(Uri endpoint, string modelId)
    {
        _serviceEndpoint = endpoint;
        _modelId = modelId;
        Metadata = new ChatClientMetadata("SampleChatClient", endpoint, modelId);
    }

    public async Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Generate a set of random responses
        var responses = new List<string>
        {
            "This is the first sample response.",
            "Here is another example of a response message.",
            "This is yet another response message."
        };

        // Choose one response randomly
        var chosenResponse = responses[_random.Next(responses.Count)];

        // Simulate some async operation
        await Task.Delay(300, cancellationToken);

        // Return a sample chat completion response
        return new ChatCompletion(
            new ChatMessage
            {
                Role = Microsoft.Extensions.AI.ChatRole.Assistant,
                Text = chosenResponse
            }
        );
    }

    public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Simulate streaming by yielding messages one by one
        yield return new StreamingChatCompletionUpdate
        {
            Role = Microsoft.Extensions.AI.ChatRole.Assistant,
            Text = "This is the first part of the stream.",
        };

        await Task.Delay(300, cancellationToken);

        yield return new StreamingChatCompletionUpdate
        {
            Role = Microsoft.Extensions.AI.ChatRole.Assistant,
            Text = "This is the second part of the stream.",
        };
    }

    public TService? GetService<TService>(object? key = null) where TService : class
    {
        // Return null as this is a sample implementation
        return this as TService;
    }

    public void Dispose()
    {
        // Clean up resources if necessary
    }
}