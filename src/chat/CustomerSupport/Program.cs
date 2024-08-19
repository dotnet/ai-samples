#pragma warning disable

// Add dependencies
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
using Azure.AI.OpenAI;
using Azure.Identity;

// Configure AI
var apiKey = "";
var endpoint = "http://localhost:11434/";

var openAIEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT");

var useOpenAI = true;
var useManagedIdentity = false;

var azureOpenAIClient =
    useManagedIdentity ?
    new OpenAIClient(new Uri(openAIEndpoint), new DefaultAzureCredential()) :
    new OpenAIClient(
        new Uri(openAIEndpoint),
        new Azure.AzureKeyCredential(Environment.GetEnvironmentVariable("AZURE_AI_KEY")));

IChatCompletionService chatService = 
    useOpenAI ? 
    new AzureOpenAIChatCompletionService("chat", azureOpenAIClient) :
    new OllamaChatCompletionService("llama3.1", endpoint);

ITextEmbeddingGenerationService embeddingService =
    useOpenAI ?
    new AzureOpenAITextEmbeddingGenerationService("embeddingsmall", azureOpenAIClient) : 
    new OllamaTextEmbeddingGenerationService("all-minilm", new Uri(endpoint));

// Ingest manuals
if(!File.Exists("./data/manual-chunks.json"))
{
    var manualIngestor = new ManualIngestor(embeddingService);
    await manualIngestor.RunAsync("./data/manuals", "./data");
}

// Load tickets and manuals
var tickets = LoadTickets("./data/tickets.json");
var manuals = LoadManualChunks("./data/manual-chunks.json");

// Service configurations
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
        // InspectTicket(tickets);

        // With AI Summaries
        // await InspectTicketWithAISummaryAsync(tickets, summaryGenerator);

        // With Semantic Search 
        await InspectTicketWithSemanticSearchAsync(tickets, summaryGenerator, productManualSearchService, chatService);

    }
}
