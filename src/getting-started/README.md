# Getting Started

These samples demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` model, from a simple .NET 8.0 console application. It consists of a simple console applications, running locally, that will send request to an Azure OpenAI service deployed in your Azure subscription. 

Everything will be deployed automatically using the Azure Developer CLI.

## Requirements

- .NET 8.0 SDK - [Install the .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An Azure subscription - [Create one for free](https://azure.microsoft.com/free)
- Azure Developer CLI - [Install or update the Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)

## Getting Started

Ensure that you follow the prerequisites to have access to Azure OpenAI Service as well as the Azure Developer CLI, and then follow the following guide to set started with the sample application.

1. Clone/ Download the repository
2. From a terminal or command prompt, navigate to the `Getting-Started` directory.

3. Create the Azure resources (Azure OpenAI service, gpt-35-turbo model) using the Azure Developer CLI:
	```bash
	azd up
	```
4. Now that your Azure OpenAI Service is deployed, It's time to select one of our sample to experiment different scenarios:
	- [Hike Summary](01-HikeSummary/README.md)
	- [Hiker AI](02-HikerAI/README.md)
	- [Chatting About my Previous Hikes](03-ChattingAboutMyHikes/README.md)
	- [AI and Native .NET](04-AiAndNative/README.md)
	- [Hike Images](05-HikeImages/README.md)

5. Once you are done delete the Azure recourse with the following command.
	```bash
	azd down
	```

## Troubleshooting

On Windows, you may get an error message: "*postprovision.ps1 is not digitally signed. The script will not execute on the system*" after the deployment. This is cause by the script "postprovision" being executed locally after the deployment to create .NET secret that will be used in the application. To avoid this error, execute the command `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`. And re-run the `azd up` command.