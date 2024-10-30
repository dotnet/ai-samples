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
                        "Dependency Injection",
                        "Text Embedding",
                        "Text Embedding Caching",
                            })
                );


        // Execute the selected sample
        switch (selectedSample)
        {
            case "Chat":
                await OpenAISamples.Chat();
                break;
            case "Conversation History":
                await OpenAISamples.ConversationHistory();
                break;
            case "Streaming":
                await OpenAISamples.Streaming();
                break;
            case "Tool Calling":
                await OpenAISamples.ToolCalling();
                break;
            case "Caching":
                await OpenAISamples.Caching();
                break;
            case "OpenTelemetry":
                await OpenAISamples.OpenTelemetryExample();
                break;
            case "Middleware":
                await OpenAISamples.Middleware();
                break;
            case "Dependency Injection":
                await OpenAISamples.DependencyInjection();
                break;
            case "Text Embedding":
                await OpenAISamples.TextEmbedding();
                break;
            case "Text Embedding Caching":
                await OpenAISamples.TextEmbeddingCaching();
                break;
        }
    }
}