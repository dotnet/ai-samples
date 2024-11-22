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
            "Chat" => OllamaSamples.Chat(),
            "Conversation History" => OllamaSamples.ConversationHistory(),
            "Streaming" => OllamaSamples.Streaming(),
            "Tool Calling" => OllamaSamples.ToolCalling(),
            "Caching" => OllamaSamples.Caching(),
            "OpenTelemetry" => OllamaSamples.OpenTelemetryExample(),
            "Middleware" => OllamaSamples.Middleware(),
            "Dependency Injection" => OllamaSamples.DependencyInjection(),
            "Text Embedding" => OllamaSamples.TextEmbedding(),
            "Text Embedding Caching" => OllamaSamples.TextEmbeddingCaching(),
            _ => Task.CompletedTask,
        });
    }
}
