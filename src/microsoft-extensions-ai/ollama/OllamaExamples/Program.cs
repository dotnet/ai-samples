using Spectre.Console;

while(true)
{
    var prompt = 
        AnsiConsole
            .Prompt(
                new SelectionPrompt<string>()
                    .Title("Enter a command")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(new[] {"Choose sample", "Quit"})
            );

    if(prompt == "Quit") break;

    if(prompt == "Choose sample")
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
                await OllamaSamples.Chat();
                break;
            case "Conversation History":
                await OllamaSamples.ConversationHistory();
                break;
            case "Streaming":
                await OllamaSamples.Streaming();
                break;
            case "Tool Calling":
                await OllamaSamples.ToolCalling();
                break;
            case "Caching":
                await OllamaSamples.Caching();
                break;
            case "OpenTelemetry":
                await OllamaSamples.OpenTelemetryExample();
                break;
            case "Middleware":
                await OllamaSamples.Middleware();
                break;
            case "Dependency Injection":
                await OllamaSamples.DependencyInjection();
                break;
            case "Text Embedding":
                await OllamaSamples.TextEmbedding();
                break;
            case "Text Embedding Caching":
                await OllamaSamples.TextEmbeddingCaching();
                break;        
        }
    }
}