using System.Text.Json.Serialization;

public class ManualChunk
{
    public int ChunkId { get; set; }

    public int ProductId { get; set; }

    public int PageNumber { get; set; }

    public required string Text { get; set; }

    public required ReadOnlyMemory<float> Embedding { get; set; }
}

public class Message
{
    public int MessageId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int TicketId { get; set; }

    public bool IsCustomerMessage { get; set; }

    public required string Text { get; set; }
}

public enum TicketStatus
{
    Open,
    Closed,
}
public class Ticket
{
    public int TicketId { get; set; }

    public int? ProductId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ShortSummary { get; set; }

    public int CustomerId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TicketStatus TicketStatus { get; set; }

    public List<Message> Messages { get; set; } = new();

}

