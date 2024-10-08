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
                        "Dependency Injection"
                            })
                );


        // Execute the selected sample
        switch (selectedSample)
        {
            case "Chat":
                await AzureAIInferenceSamples.Chat();
                break;
            case "Conversation History":
                await AzureAIInferenceSamples.ConversationHistory();
                break;
            case "Streaming":
                await AzureAIInferenceSamples.Streaming();
                break;
            case "Tool Calling":
                await AzureAIInferenceSamples.ToolCalling();
                break;
            case "Caching":
                await AzureAIInferenceSamples.Caching();
                break;
            case "OpenTelemetry":
                await AzureAIInferenceSamples.OpenTelemetryExample();
                break;
            case "Middleware":
                await AzureAIInferenceSamples.Middleware();
                break;
            case "Dependency Injection":
                await AzureAIInferenceSamples.DependencyInjection();
                break;
        }
    }
}