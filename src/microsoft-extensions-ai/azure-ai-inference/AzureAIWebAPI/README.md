# Microsoft.Extensions.AI - Azure AI Inference Web API Example

This project contains a minimal Web API that show how to use the Azure AI Inference reference implementation in the [Microsoft.Extensions.AI.AzureAIInference NuGet package](https://aka.ms/meai-azaiinference-nuget).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://visualstudio.microsoft.com/downloads/)
- A GitHub Personal Access Token. For more details, see the [GitHub documentation](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens).

## Setup

1. In the *AzureAIInferenceWebAPI* project directory, create a file called *appsettings.local.json* with the following content.

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
        "AzureAIInference": {
            "Key": "YOUR-GH-PAT-TOKEN",
            "Chat": {
                "Endpoint": "https://models.inference.ai.azure.com",
                "ModelId": "gpt-4o-mini"
            }
        }
    }
    }
    ```

1. Replace the value of the `Key` with your GitHub Personal Access Token.

## Quick Start

### Visual Studio

1. Open the *AzureAIInferenceExamples.sln* solution
1. Set *AzureAIInferenceWebAPI* as the startup project.
1. Press <kbd>F5</kbd>.

### Visual Studio Code

1. Open your terminal
1. Navigate to the *AzureAIInferenceWebAPI* project directory
1. Run the applicaton using `dotnet run`

    ```dotnetcli
    dotnet run
    ```

## Test your application

### PowerShell

##### Chat

```powershell
$response = Invoke-RestMethod -Uri 'http://localhost:5093/chat' -Method Post -Headers @{'Content-Type'='application/json'} -Body '"What is AI?"'; $response.message.contents.text
```