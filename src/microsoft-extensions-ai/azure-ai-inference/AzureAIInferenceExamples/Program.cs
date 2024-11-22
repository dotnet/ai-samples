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
                        ])
                );


        // Execute the selected sample
        await (selectedSample switch
        {
            "Chat" => AzureAIInferenceSamples.Chat(),
            "Conversation History" => AzureAIInferenceSamples.ConversationHistory(),
            "Streaming" => AzureAIInferenceSamples.Streaming(),
            "Tool Calling" => AzureAIInferenceSamples.ToolCalling(),
            "Caching" => AzureAIInferenceSamples.Caching(),
            "OpenTelemetry" => AzureAIInferenceSamples.OpenTelemetryExample(),
            "Middleware" => AzureAIInferenceSamples.Middleware(),
            "Dependency Injection" => AzureAIInferenceSamples.DependencyInjection(),
            "Text Embedding" => AzureAIInferenceSamples.TextEmbedding(),
            "Text Embedding Caching" => AzureAIInferenceSamples.TextEmbeddingCaching(),
            _ => Task.CompletedTask,
        });
    }
}
