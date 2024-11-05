#pragma warning disable
using Microsoft.SemanticKernel.Connectors.InMemory;

// Configure AI
var ollamaEndpoint = "http://localhost:11434/";
var openAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

var useOpenAIChat = true; // Use OpenAI chat completion models
var useOpenAIEmbeddings = true; // Use OpenAI text embedding generation models
var useManagedIdentity = true;

IChatClient chatClient =
    useOpenAIChat ?
    Utils.CreateAzureOpenAIClient(openAIEndpoint, useManagedIdentity)
        .AsChatClient("chat")
    : new OllamaApiClient(new Uri(ollamaEndpoint), "llama3.2");

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    useOpenAIEmbeddings ?
        Utils.CreateAzureOpenAIClient(openAIEndpoint, useManagedIdentity)
            .AsEmbeddingGenerator("embeddingsmall") :
                new OllamaApiClient(new Uri(ollamaEndpoint), "all-minilm");

// Configure product manual service
var vectorStore = new InMemoryVectorStore();
var productManualService = new ProductManualService(embeddingGenerator, vectorStore, useOpenAIEmbeddings);
// Ingest manuals

if (!File.Exists("./data/manual-chunks.json"))
{
    var manualIngestor = new ManualIngestor(embeddingGenerator);
    await manualIngestor.RunAsync("./data/manuals", "./data");
}

// Load tickets and manuals
var tickets = LoadTickets("./data/tickets.json");
LoadManualsIntoVectorStore("./data/manual-chunks.json", productManualService);

// Service configurations
var summaryGenerator = new TicketSummarizer(chatClient);

while (true)
{
    var prompt =
        AnsiConsole
            .Prompt(
                new SelectionPrompt<string>()
                    .Title("Enter a command")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(new[] { "Inspect ticket", "Quit" })
            );

    if (prompt == "Quit") break;

    if (prompt == "Inspect ticket")
    {
        // No AI
        // InspectTicket(tickets);

        // With AI Summaries
        // await InspectTicketWithAISummaryAsync(tickets, summaryGenerator);

        // With Semantic Search 
        await InspectTicketWithSemanticSearchAsync(tickets, summaryGenerator, productManualService, chatClient);
    }
}
