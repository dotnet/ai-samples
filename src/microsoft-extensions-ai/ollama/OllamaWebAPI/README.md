# Microsoft.Extensions.AI - Ollama Web API Example

This project contains a minimal Web API that show how to use the Ollama reference implementation in the [Microsoft.Extensions.AI.Ollama NuGet package](https://aka.ms/meai-ollama-nuget).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://visualstudio.microsoft.com/downloads/)
- [Ollama](https://ollama.com/download)

## Setup

1. Open your terminal and download the following models using Ollama.

    ```bash
    ollama pull llama3.1 // chat
    ollama pull all-minilm // embeddings
    ```

1. In the *OllamaWebAPI* project directory, create a file called *appsettings.local.json* with the following content.

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
            "Ollama": {
                "Chat": {
                    "Endpoint": "http://localhost:11434/",
                    "ModelId": "llama3.1"
                },
                "Embedding": {
                    "Endpoint": "http://localhost:11434/",
                    "ModelId": "all-minilm"
                }
            }
        }
    }
    ```

## Quick Start

### Visual Studio

1. Open the *OllamaExamples.sln* solution
1. Set *OllamaWebAPI* as the startup project.
1. Press <kbd>F5</kbd>.

### Visual Studio Code

1. Open your terminal
1. Navigate to the *OllamaWebAPI* project directory
1. Run the applicaton using `dotnet run`

    ```dotnetcli
    dotnet run
    ```

## Test your application

### PowerShell

##### Chat

```powershell
$response = Invoke-RestMethod -Uri 'http://localhost:5078/chat' -Method Post -Headers @{'Content-Type'='application/json'} -Body '"What is AI?"'; $response.message.contents.text
```

#### Embeddings

```powershell
$response = Invoke-RestMethod -Uri 'http://localhost:5078/embedding' -Method Post -Headers @{'Content-Type'='application/json'} -Body '"What is AI?"'; $response.vector
```