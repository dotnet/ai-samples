# Customer Support 

This sample is a C# console application that uses local models provided by Ollama to:

- Generate customer support ticket summaries
- Generate embeddings for product manuals
- Perform semantic search over product manuals

## Project structure

- *OllamaTextEmbeddingGeneration.cs* - Embedding generation service that uses Ollama models. 
- *ManualIngestor.cs* - Ingestion service that extracts data from product manual PDF files, chunks text into smaller segments, generates embeddings, saves them to a JSON file.
- *ProductManualSemanticSearch.cs* - Search service that uses product manual embeddings for semantic search. 
- *TicketSummarizer* - AI service which uses an AI model to generate summaries of customer support tickets.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Ollama](https://ollama.com/download)
- (Optional) Azure subscription. [Create one for free](https://azure.microsoft.com/free/cognitive-services?azure-portal=true).


## Configuration

For this application, you can either use local language and embedding models with Ollama or models hosted on Azure OpenAI. 

### Ollama

1. Download [llama3.1](https://ollama.com/library/llama3.1) language model

    ```bash
    ollama pull llama3.1
    ```

1. Download [all-minilm](https://ollama.com/library/all-minilm) embedding model

    ```bash
    ollama pull all-minilm
    ```

1. In *Program.cs*, set `useOpenAI` to `false`.

### Azure OpenAI

1. Deploy a chat and embedding model. For more details, [see the Azure OpenAI documentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model).
    
    - **chat** - A language model that supports chat. (i.e. `gpt-35-turbo`)
    - **embedding** - An embedding model. (i.e. `text-embedding-3-small`)

    See the Azure OpenAI documentation for a full [list of models available](https://learn.microsoft.com/azure/ai-services/openai/concepts/models). 

    If you use deployment names other than *chat* and *embedding*, update them in `Program.cs`;

1. Configure environment variables for your endpoint and key. For more details, [see the Azure OpenAI documentation](https://learn.microsoft.com/azure/ai-services/openai/chatgpt-quickstart?tabs=command-line%2Cpython-new&pivots=programming-language-csharp#retrieve-key-and-endpoint).
    - **AZURE_AI_ENDPOINT** - Your Azure OpenAI endpoint.
    - **AZURE_AI_KEY** - Your Azure OpenAI Key.
1. In *Program.cs*, set `useOpenAI` to `true`.