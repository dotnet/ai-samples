#pragma warning disable

// Part 0: Add dependencies
// Microsoft.Semantic Kernel
// using Microsoft.Extensions.DependencyInjection;

using System.Text.Json;
using Spectre.Console;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Collections.Immutable;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using static Utils;
using Microsoft.SemanticKernel.Embeddings;
// using Microsoft.SemanticKernel.Embeddings;

// Configure AI
var apiKey = "";
var endpoint = new Uri("http://localhost:11434/");

var openAIKey = Environment.GetEnvironmentVariable("AZURE_AI_KEY");
var openAIEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT");

var useOpenAI = false;

IChatCompletionService chatService = 
    useOpenAI ? 
    new AzureOpenAIChatCompletionService("chat", openAIEndpoint, openAIKey) :
    new OpenAIChatCompletionService("llama3.1", endpoint, apiKey);

ITextEmbeddingGenerationService embeddingService =
    useOpenAI ?
    new AzureOpenAITextEmbeddingGenerationService("embeddingsmall", openAIEndpoint, openAIKey) : 
    new OllamaTextEmbeddingGenerationService("all-minilm", endpoint);

// Part 0: Ingest manuals
if(!File.Exists("./data/manual-chunks.json"))
{
    var manualIngestor = new ManualIngestor(embeddingService);
    await manualIngestor.RunAsync("./data/manuals", "./data");
}

// Part 1: Load tickets and manuals
var tickets = LoadTickets("./data/tickets.json");
var manuals = LoadManualChunks("./data/manual-chunks.json");

// Part 2: Service
var summaryGenerator = new TicketSummarizer(chatService);
var productManualSearchService = new ProductManualSemanticSearch(embeddingService, manuals);

while(true)
{
    var prompt = 
        AnsiConsole
            .Prompt(
                new SelectionPrompt<string>()
                    .Title("Enter a command")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(new[] {"Inspect ticket", "Quit"})
            );

    if(prompt == "Quit") break;

    if(prompt == "Inspect ticket")
    {
        // No AI
        InspectTicket(tickets);

        // With AI Summaries
        // await InspectTicketWithAISummaryAsync(tickets, summaryGenerator);

        // With Semantic Search 
        // await InspectTicketWithSemanticSearchAsync(tickets, summaryGenerator, productManualSearchService, chatService);

    }
}
