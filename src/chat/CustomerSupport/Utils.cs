using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualBasic;
using Spectre.Console;

public static class Utils
{

    public static IEnumerable<Ticket> LoadTickets(string path, int limit = 10)
    {
        var ticketData = File.ReadAllText(path);
        var tickets = JsonSerializer.Deserialize<List<Ticket>>(ticketData);
        return tickets.Take(limit);
    }

    public static void SaveTickets(string path, IEnumerable<Ticket> tickets)
    {
        var ticketData = JsonSerializer.Serialize(tickets);
        File.WriteAllText(path, ticketData);
    }

    public static IEnumerable<ManualChunk> LoadManualChunks(string path)
    {
        var chunkData = File.ReadAllText(path);
        var chunks = JsonSerializer.Deserialize<List<ManualChunk>>(chunkData);
        return chunks;
    }

    public static void InspectTicket(IEnumerable<Ticket> tickets)
    {
        // User selects ticket
        var ticket =
            AnsiConsole.Prompt(
                new SelectionPrompt<Ticket>()
                    .Title("Select ticket")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(tickets)
                    .UseConverter(ticket => $"{ticket.TicketId.ToString()} - {ticket.ShortSummary}")
            );

        if (ticket == null)
        {
            Console.WriteLine("Ticket not found.");
        }

        // Tickets formatted for display
        var formattedMessages = ticket.Messages.Select(m =>
            m.IsCustomerMessage ? $"[blue]Customer: {m.Text}[/]" : $"[green]Agent: {m.Text}[/]");
        var messageText = string.Join("\n", formattedMessages);

        // Display tickets
        var panel = new Panel(messageText);
        panel.Header = new PanelHeader($"Customer Messages for Ticket ID: {ticket.TicketId}");
        AnsiConsole.Write(panel);
    }

    public static async Task InspectTicketWithAISummaryAsync(IEnumerable<Ticket> tickets, TicketSummarizer summaryGenerator)
    {
        // User selects ticket
        var ticket =
            AnsiConsole.Prompt(
                new SelectionPrompt<Ticket>()
                    .Title("Select ticket")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(tickets)
                    .UseConverter(ticket => $"{ticket.TicketId.ToString()} - {ticket.ShortSummary}")
            );

        if (ticket == null)
        {
            Console.WriteLine("Ticket not found.");
        }

        // Tickets formatted for display
        var formattedMessages = ticket.Messages.Select(m =>
            m.IsCustomerMessage ? $"[blue]Customer: {m.Text}[/]" : $"[green]Agent: {m.Text}[/]");
        var messageText = string.Join("\n", formattedMessages);

        // Generate summary
        var summary = await summaryGenerator.GenerateLongSummaryAsync(messageText);

        // Display tickets
        var panel = new Panel($"[olive]Summary: {summary}[/]\n\n{messageText}");
        panel.Header = new PanelHeader($"Customer Messages for Ticket ID: {ticket.TicketId}");
        AnsiConsole.Write(panel);
    }

    public static async Task InspectTicketWithSemanticSearchAsync(IEnumerable<Ticket> tickets, TicketSummarizer summaryGenerator, ProductManualSemanticSearch productManualSearchService, IChatCompletionService chatService)
    {
        // User selects ticket
        var ticket =
            AnsiConsole.Prompt(
                new SelectionPrompt<Ticket>()
                    .Title("Select ticket")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(tickets)
                    .UseConverter(ticket => $"{ticket.TicketId.ToString()} - {ticket.ShortSummary}")
            );

        if (ticket == null)
        {
            Console.WriteLine("Ticket not found.");
        }

        // Tickets formatted for display
        var formattedMessages = ticket.Messages.Select(m =>
            m.IsCustomerMessage ? $"[blue]Customer: {m.Text}[/]" : $"[green]Agent: {m.Text}[/]");
        var messageText = string.Join("\n", formattedMessages);

        // Generate summary
        var summary = await summaryGenerator.GenerateLongSummaryAsync(messageText);

        var panel = new Panel($"[olive]Summary: {summary}[/]\n\n{messageText}");
        panel.Header = new PanelHeader($"Customer Messages for Ticket ID: {ticket.TicketId}");
        AnsiConsole.Write(panel);

        // Chat loop
        var prompt = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Enter a command")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                .AddChoices(new[] { "Chat", "Back" })
        );

        if (prompt == "Back") return;

        if (prompt == "Chat")
        {
            while (true)
            {
                var query =
                    AnsiConsole
                        .Prompt(
                            new TextPrompt<string>("Enter a message (type 'quit' to exit)")
                                .PromptStyle("green")
                        );

                if (query == "quit") break;

                // RAG loop
                // [1] Search for relevant documents
                var searchResults = await productManualSearchService.SearchAsync(ticket.ProductId.Value, query);

                // [2] Augment prompt with search results
                var message = $"""
                Using the following data sources as context, answer the user query: {query}
                
                ## Context
                {String.Join("\n", searchResults)}

                Response: 
                """;

                // [3] Generate response
                var response = await chatService.GetChatMessageContentAsync(message);

                AnsiConsole.MarkupLine($"[bold yellow]{response}[/]");
            }
        }
    }
}
