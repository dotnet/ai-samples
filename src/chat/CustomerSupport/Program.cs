#pragma warning disable

// Add dependencies
using System.Text.Json;
using Spectre.Console;
using System.Collections.Immutable;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using static Utils;
using Microsoft.SemanticKernel.Embeddings;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using System.ClientModel;
using Microsoft.SemanticKernel.Connectors.InMemory;

// Configure AI
var ollamaEndpoint = "http://localhost:11434/";
var openAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

var useOpenAI = true;
var useManagedIdentity = true;

IChatClient chatClient =
    useOpenAI ?
    Utils.CreateAzureOpenAIClient(openAIEndpoint, useManagedIdentity)
        .AsChatClient("chat")
    : new OllamaChatClient(new Uri(ollamaEndpoint), "llama3.1");

IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator =
    useOpenAI ?
        Utils.CreateAzureOpenAIClient(openAIEndpoint, useManagedIdentity)
            .AsEmbeddingGenerator("embeddingsmall") :
                new OllamaEmbeddingGenerator(new Uri(ollamaEndpoint), "all-minilm");

// Configure product manual service
var vectorStore = new InMemoryVectorStore();
var productManualService = new ProductManualService(embeddingGenerator, vectorStore);
// Ingest manuals

if(!File.Exists("./data/manual-chunks.json"))
{
    var manualIngestor = new ManualIngestor(embeddingGenerator);
    await manualIngestor.RunAsync("./data/manuals", "./data");
}

// Load tickets and manuals
var tickets = LoadTickets("./data/tickets.json");
LoadManualsIntoVectorStore("./data/manual-chunks.json", productManualService);

// Service configurations
var summaryGenerator = new TicketSummarizer(chatClient);

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
        await InspectTicketWithSemanticSearchAsync(tickets, summaryGenerator, productManualService, chatClient);

    }
}
