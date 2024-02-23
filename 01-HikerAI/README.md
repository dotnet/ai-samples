# HikerAI

This sample demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` model, from a simple .NET 8.0 console application. Get a hiking recommendation from the AI model. It consists of a simple console application,running locally, that will send request to an Azure OpenAI service deployed in your Azure subscription. 

Everything will be deployed automatically using the Azure Developer CLI.

## Requirements

- .NET 8.0 SDK - [Install the .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An Azure subscription - [Create one for free](https://azure.microsoft.com/free)
- Azure Developer CLI - [Install or update the Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)

## Getting Started

Ensure that you follow the prerequisites to have access to Azure OpenAI Service as well as the Azure Developer CLI, and then follow the following guide to set started with the sample application.

1. Clone/ Download the repository
1. From a terminal or command prompt, navigate to the `01-HikerAI` directory.

2. Create the Azure resources (Azure OpenAI service, gpt-35-turbo model) using the Azure Developer CLI:
	```bash
	azd up
	```
3. It's now time to try the console application. Depending on your Azure subscription it's possible that a few (~5) minutes more minute are required before the model deployed in Azure OpenAI get available. If you get an error message about this, wait a few minutes and try again.
	```bash
	dotnet run
	```
4. Once you are done delete the Azure recourse with the following command.
	```bash
	azd down
	```

## Troubleshooting

On Windows, you may get an error message: "*postprovision.ps1 is not digitally signed. The script will not execute on the system*" after the deployment. This is cause by the script "postprovision" being executed locally after the deployment to create .NET secret that will be used in the application. To avoid this error, execute the command `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`. And re-run the `azd up` command.
