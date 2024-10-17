# Microsoft.Extensions.AI - Azure OpenAI Web API Example

This project contains a minimal Web API that show how to use the OpenAI reference implementation in the [Microsoft.Extensions.AI.OpenAI NuGet package](https://aka.ms/meai-openai-nuget) with the Azure OpenAI service.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://visualstudio.microsoft.com/downloads/)
- An Azure OpenAI Service resource with a chat completion and text embedding generation model deployments. For more details, see the [Azure OpenAI resource deployment documentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource).

## Setup

1. In the *AzureOpenAIWebAPI* project directory, create a file called *appsettings.local.json* with the following content.

    ```json
    {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }
        },
        "AllowedHosts": "*",
        "AI": {
            "AzureOpenAI": {
                "Endpoint": "YOUR-AZURE-OPENAI-ENDPOINT",
                "Chat": {
                    "ModelId": "gpt-4o-mini"
                },
                "Embedding": {
                    "ModelId": "text-embedding-3-small"
                }
            }
        }
    }
    ```

1. Replace the value of the `Endpoint` with your Azure OpenAI API endpoint. For more details on where to find your Azure OpenAI endpoint, see the [Azure OpenAI documentation](https://learn.microsoft.com/azure/ai-services/openai/chatgpt-quickstart?tabs=command-line%2Ctypescript%2Cpython-new&pivots=programming-language-csharp#retrieve-key-and-endpoint). 

## Quick Start

### Visual Studio

1. Open the *AzureOpenAIExamples.sln* solution
1. Set *AzureOpenAIWebAPI* as the startup project.
1. Press <kbd>F5</kbd>.

### Visual Studio Code

1. Open your terminal
1. Navigate to the *AzureOpenAIWebAPI* project directory
1. Run the applicaton using `dotnet run`

    ```dotnetcli
    dotnet run
    ```

## Test your application

### PowerShell

##### Chat

```powershell
$response = Invoke-RestMethod -Uri 'http://localhost:5208/chat' -Method Post -Headers @{'Content-Type'='application/json'} -Body '"What is AI?"'; $response.message.contents.text
```

#### Embeddings

```powershell
$response = Invoke-RestMethod -Uri 'http://localhost:5208/embedding' -Method Post -Headers @{'Content-Type'='application/json'} -Body '"What is AI?"'; $response.vector
```