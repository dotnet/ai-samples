# Microsoft.Extensions.AI - OpenAI Web API Example

This project contains a minimal Web API that show how to use the OpenAI reference implementation in the [Microsoft.Extensions.AI.OpenAI NuGet package](https://aka.ms/meai-openai-nuget).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://visualstudio.microsoft.com/downloads/)
- An Open AI API key. For more details, see the [OpenAI documentation](https://help.openai.com/en/articles/4936850-where-do-i-find-my-openai-api-key).

## Setup

1. In the *OpenAIWebAPI* project directory, create a file called *appsettings.local.json* with the following content.

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
            "OpenAI": {
                "Key": "YOUR-API-KEY",
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

1. Replace the value of the `Key` with your OpenAI API key.

## Quick Start

### Visual Studio

1. Open the *OpenAIExamples.sln* solution
1. Set *OpenAIWebAPI* as the startup project.
1. Press <kbd>F5</kbd>.

### Visual Studio Code

1. Open your terminal
1. Navigate to the *OpenAIWebAPI* project directory
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