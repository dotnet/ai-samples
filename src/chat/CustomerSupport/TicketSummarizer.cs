public class TicketSummarizer
{
    private readonly IChatClient _chatClient;

    public TicketSummarizer(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    private string GetShortSummaryPrompt(string messages)
    {
        return
            $"""
            You are part of a customer support ticketing system.
            
            Your job is to write brief summaries of customer support interactions.

            This is to help support agents understand the context quickly so they can help the customer efficiently.

            Here are details of a support ticket.

            ${messages}

            Write a summary that is up to 10 words long of the latest thing the CUSTOMER has said, ignoring any agent messages.

            Summary: 
            """;
    }

    private string GetLongSummaryPrompt(string messages)
    {
        return
            $"""
            You are part of a customer support ticketing system.
            
            Your job is to write brief summaries of customer support interactions.

            This is to help support agents understand the context quickly so they can help the customer efficiently.

            Here are details of a support ticket.

            ${messages}

            Write a summary that is up to 30 words long, condensing as much distinctive information as possible.

            Summary: 
            """;
    }

    public async Task<ChatCompletion> GenerateLongSummaryAsync(string input)
    {
        var prompt = GetLongSummaryPrompt(input);
        var response = await _chatClient.CompleteAsync(prompt);
        return response;
    }

    public async Task<ChatCompletion> GenerateShortSummaryAsync(string input)
    {
        var prompt = GetShortSummaryPrompt(input);
        var response = await _chatClient.CompleteAsync(prompt);
        return response;
    }
}
