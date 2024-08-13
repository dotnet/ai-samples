using System.Collections.ObjectModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using Spectre.Console;

public class OllamaChatCompletionService : IChatCompletionService
{
    private readonly OllamaApiClient _client;

    public OllamaChatCompletionService(
        string modelId,
        string endpoint)
    {
        _client = new OllamaApiClient(endpoint, modelId);
    }

    public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var settings = OllamaPromptExecutionSettings.FromExecutionSettings(executionSettings);
        var request = CreateChatRequest(chatHistory, settings, _client.SelectedModel);

        var response = await this._client.Chat(request, cancellationToken).ConfigureAwait(false);

        return [new ChatMessageContent(
            role: GetAuthorRole(response.Message.Role) ?? AuthorRole.Assistant,
            content: response.Message.Content,
            modelId: response.Model,
            innerContent: response,
            metadata: new OllamaMetadata(response))];
    }

    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static AuthorRole? GetAuthorRole(OllamaSharp.Models.Chat.ChatRole? role) => role.ToString().ToUpperInvariant() switch
    {
        "USER" => AuthorRole.User,
        "ASSISTANT" => AuthorRole.Assistant,
        "SYSTEM" => AuthorRole.System,
        _ => null
    };
    private static ChatRequest CreateChatRequest(ChatHistory chatHistory, OllamaPromptExecutionSettings? settings, string selectedModel)
    {
        var messages = new List<OllamaSharp.Models.Chat.Message>();
        foreach (var chatHistoryMessage in chatHistory)
        {
            OllamaSharp.Models.Chat.ChatRole role = OllamaSharp.Models.Chat.ChatRole.User;
            if (chatHistoryMessage.Role == AuthorRole.System)
            {
                role = OllamaSharp.Models.Chat.ChatRole.System;
            }
            else if (chatHistoryMessage.Role == AuthorRole.Assistant)
            {
                role = OllamaSharp.Models.Chat.ChatRole.Assistant;
            }

            messages.Add(new OllamaSharp.Models.Chat.Message(role, chatHistoryMessage.Content!));
        }

        var request = new ChatRequest
        {
            Options = new()
            {
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                TopK = settings.TopK,
                Stop = settings.Stop?.ToArray()
            },
            Messages = messages.ToList(),
            Model = selectedModel,
            Stream = true
        };
        return request;
    }
}

class OllamaPromptExecutionSettings : PromptExecutionSettings
{
    public float Temperature { get; set; }
    public float TopP { get; set; }
    public int TopK { get; set; }
    public string[]? Stop { get; set; }

    public static OllamaPromptExecutionSettings FromExecutionSettings(PromptExecutionSettings? executionSettings)
    {
        switch (executionSettings)
        {
            case null:
                return new();
            case OllamaPromptExecutionSettings settings:
                return settings;
        }

        var json = JsonSerializer.Serialize(executionSettings);
        var ollamaExecutionSettings = JsonSerializer.Deserialize<OllamaPromptExecutionSettings>(json);
        if (ollamaExecutionSettings is not null)
        {
            return ollamaExecutionSettings;
        }

        return ollamaExecutionSettings;
    }
}

class OllamaMetadata : ReadOnlyDictionary<string, object?>
{

    public OllamaMetadata(ChatResponse response) : base(new Dictionary<string, object?>())
    {
        this.TotalDuration = response.TotalDuration;
        this.EvalCount = response.EvalCount;
        this.EvalDuration = response.EvalDuration;
        this.CreatedAt = response.CreatedAt;
        this.LoadDuration = response.LoadDuration;
        this.PromptEvalDuration = response.PromptEvalDuration;
    }

    public long TotalDuration { get; private set; }
    public int EvalCount { get; set; }
    public long EvalDuration { get; set; }
    public string CreatedAt { get; set; }
    public long LoadDuration { get; set; }
    public long PromptEvalDuration { get; set; }
}
