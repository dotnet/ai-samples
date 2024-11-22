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
                    .AddChoices(["Choose sample", "Quit"])
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
                        .AddChoices([
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
                         ])
                );

        // Execute the selected sample
        await (selectedSample switch
        {
            "Chat" => AbstractionSamples.Chat(),
            "Conversation History" => AbstractionSamples.ConversationHistory(),
            "Streaming" => AbstractionSamples.Streaming(),
            "Tool Calling" => AbstractionSamples.ToolCalling(),
            "Caching" => AbstractionSamples.Caching(),
            "OpenTelemetry" => AbstractionSamples.OpenTelemetryExample(),
            "Middleware" => AbstractionSamples.Middleware(),
            "Text Embedding" => AbstractionSamples.TextEmbedding(),
            "Text Embedding Caching" => AbstractionSamples.TextEmbeddingCaching(),
            "Logging Chat" => AbstractionSamples.LoggingChat(),
            "Logging Embedding" => AbstractionSamples.LoggingEmbedding(),
            "Dependency Injection" => AbstractionSamples.DependencyInjection(),
            _ => Task.CompletedTask,
        });
    }
}
