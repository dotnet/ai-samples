# Customer Support 

This sample is a C# console application that uses Generative AI services like Ollama or Azure OpenAI to:

- Generate customer support ticket summaries
- Generate embeddings for product manuals
- Perform semantic search over product manuals

## Project structure

- *ManualIngestor.cs* - Ingestion service that extracts data from product manual PDF files, chunks text into smaller segments, generates embeddings, saves them to a JSON file.
- *ProductManualService.cs* - Storage service that uses an `IVectorStore` to save and search product manual embeddings.
- *TicketSummarizer* - AI service which uses an AI model to generate summaries of customer support tickets.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Ollama](https://ollama.com/download)
- (Optional) Azure subscription. [Create one for free](https://azure.microsoft.com/free/cognitive-services?azure-portal=true).

## Quick Start

Get started setting up your environment using either GitHub Codespaces or Dev Containers.

| Environment | Codespaces | DevContainer |
| --- | --- | --- |
| GPU (Recommended) | N/A | Coming Soon | 
| CPU | [![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/luisquintanilla/ai-samples/tree/customer-support-sample?devcontainer_path=.devcontainer%2Follama-cpu%2Fdevcontainer.json&machine=premiumLinux) | Coming soon |

## Configuration

For this application, you can either use local language and embedding models with Ollama or models hosted on Azure OpenAI. 

### Ollama

1. Download [llama3.2](https://ollama.com/library/llama3.2) language model

    ```bash
    ollama pull llama3.2
    ```

1. Download [all-minilm](https://ollama.com/library/all-minilm) embedding model

    ```bash
    ollama pull all-minilm
    ```

1. In *Program.cs*, depending on what you want to use the model for set:

    - `useOpenAIChat` to `false`
    - `useOpenAIEmbeddings` to `false`

### Azure OpenAI

1. Deploy a chat and embedding model. For more details, [see the Azure OpenAI documentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model).
    
    - **chat** - A language model that supports chat. (i.e. `gpt-35-turbo`)
    - **embedding** - An embedding model. (i.e. `text-embedding-3-small`)

    See the Azure OpenAI documentation for a full [list of models available](https://learn.microsoft.com/azure/ai-services/openai/concepts/models). 

    If you use deployment names other than *chat* and *embedding*, update them in `Program.cs`;

1. In *Program.cs*, depending on what you want to use the model for set: 

    - `useOpenAIChat` to `true`
    - `useOpenAIEmbeddings` to `true`

1. Configure environment variables for your endpoint and key. For more details, [see the Azure OpenAI documentation](https://learn.microsoft.com/azure/ai-services/openai/chatgpt-quickstart?tabs=command-line%2Cpython-new&pivots=programming-language-csharp#retrieve-key-and-endpoint).
    - **AZURE_OPENAI_ENDPOINT** - Your Azure OpenAI endpoint.
    - **AZURE_OPENAI_KEY** - Your Azure OpenAI Key.

#### Using managed identity (Recommended)

By using a managed identity from Microsoft Entra, your application can easily access protected Azure OpenAI resources without having to manually provision or rotate any secrets.

1. Assign Azure OpenAI user role to your managed identity. For more details, see [the documentation](https://learn.microsoft.com/dotnet/ai/how-to/app-service-aoai-auth?pivots=azure-portal#add-an-azure-openai-user-role-to-your-managed-identity).  
1. In *Program.cs*, set `useManagedIdentity` to `true`.