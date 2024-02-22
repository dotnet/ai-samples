# HikerAI

This sample demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` model, from a simple .NET 8.0 console application. Get a hiking recommendation from the AI model. It consists of a simple console application,running locally, that will send request to an Azure OpenAI service deployed in your Azure subscription. 

Everything will be deployed automatically using the Azure Developer CLI.

## Requirements

- .NET 8.0 SDK - [Install the .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0?WT.mc_id=dotnet-0000)
- An Azure subscription - [Create one for free](https://azure.microsoft.com/free/?WT.mc_id=dotnet-0000)
- Azure Developer CLI - [Install or update the Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd?WT.mc_id=dotnet-0000)

## Getting Started

Ensure that you follow the prerequisites to have access to Azure OpenAI Service as well as the Azure Developer CLI, and then follow the following guide to set started with the sample application.

1. Clone/ Download the repository
1. From a terminal or command prompt, navigate to the `HikerAI` directory.
1. To avoid an error message "*postprovision.ps1 is not digitally signed. The script will not execute on the system*" after the deployment, execute the command `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`. The script "postprovision" is executed locally after the deployment to create .NET secret that will be used in the application.
1. Create the Azure resources (Azure OpenAI service, gpt-35-turbo model, Azure KeyVault) using the Azure Developer CLI:
	```bash
	azd up
	```
2. It's now time to try the console application. Depending on your Azure subscription it's possible that a few (~5) minutes more minute are required before the model deployed in Azure OpenAI get available. If you get an error message about this, wait a few minutes and try again.
	```bash
	dotnet run
	```
3. Once you are done delete the Azure recourse with the following command.
	```bash
	azd down
	```

