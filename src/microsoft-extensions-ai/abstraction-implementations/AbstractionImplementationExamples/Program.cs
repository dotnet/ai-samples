using Spectre.Console;

while (true)
{
    var prompt =
        AnsiConsole
            .Prompt(
                new SelectionPrompt<string>()
                    .Title("Enter a command")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(new[] { "Choose sample", "Quit" })
            );

    if (prompt == "Quit") break;

    if (prompt == "Choose sample")
    {
        var selectedSample =
            AnsiConsole
                .Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose a sample")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                        .AddChoices(new[]
                            {
                        "Chat",
                        "Conversation History",
                        "Streaming",
                        "Tool Calling",
                        "Caching",
                        "OpenTelemetry",
                        "Middleware",
                        "Text Embedding",
                        "Text Embedding Caching",
                        "Logging Chat",
                        "Logging Embedding",
                        "Dependency Injection"
                            })
                );


        // Execute the selected sample
        switch (selectedSample)
        {
            case "Chat":
                await AbstractionSamples.Chat();
                break;
            case "Conversation History":
                await AbstractionSamples.ConversationHistory();
                break;
            case "Streaming":
                await AbstractionSamples.Streaming();
                break;
            case "Tool Calling":
                await AbstractionSamples.ToolCalling();
                break;
            case "Caching":
                await AbstractionSamples.Caching();
                break;
            case "OpenTelemetry":
                await AbstractionSamples.OpenTelemetryExample();
                break;
            case "Middleware":
                await AbstractionSamples.Middleware();
                break;
            case "Text Embedding":
                await AbstractionSamples.TextEmbedding();
                break;
            case "Text Embedding Caching":
                await AbstractionSamples.TextEmbeddingCaching();
                break;
            case "Logging Chat":
                await AbstractionSamples.LoggingChat();
                break;
            case "Logging Embedding":
                await AbstractionSamples.LoggingEmbedding();
                break;
            case "Dependency Injection":
                await AbstractionSamples.DependencyInjection();
                break;
        }
    }
}