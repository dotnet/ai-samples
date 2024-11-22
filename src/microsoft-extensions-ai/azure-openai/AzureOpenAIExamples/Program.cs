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
                            "Dependency Injection",
                            "Text Embedding",
                            "Text Embedding Caching",
                        ])
                );


        // Execute the selected sample
        await (selectedSample switch
        {
            "Chat" => OpenAISamples.Chat(),
            "Conversation History" => OpenAISamples.ConversationHistory(),
            "Streaming" => OpenAISamples.Streaming(),
            "Tool Calling" => OpenAISamples.ToolCalling(),
            "Caching" => OpenAISamples.Caching(),
            "OpenTelemetry" => OpenAISamples.OpenTelemetryExample(),
            "Middleware" => OpenAISamples.Middleware(),
            "Dependency Injection" => OpenAISamples.DependencyInjection(),
            "Text Embedding" => OpenAISamples.TextEmbedding(),
            "Text Embedding Caching" => OpenAISamples.TextEmbeddingCaching(),
            _ => Task.CompletedTask,
        });
    }
}
