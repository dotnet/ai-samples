# Getting Started

These samples demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` and `dall-e-3` models, from a simple .NET 8.0 console application. It consists of a simple console applications, running locally, that will send request to an Azure OpenAI service deployed in your Azure subscription. 

Everything will be deployed automatically using the Azure Developer CLI.


## Prerequisites

- .NET 8.0 SDK - [Install the .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An Azure subscription - [Create one for free](https://azure.microsoft.com/free)
- Azure Developer CLI - [Install or update the Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- Access to [Azure OpenAI service](https://learn.microsoft.com/azure/ai-services/openai/overview#how-do-i-get-access-to-azure-openai).


## Deploying the Azure resources

Ensure that you follow the [Prerequisites](#prerequisites) to have access to Azure OpenAI Service as well as the Azure Developer CLI, and then follow the following guide to set started with the sample application.

1. Clone/ Download the repository
   
2. From a terminal or command prompt, navigate to the `Getting-Started` directory.

3. Create the Azure resources (Azure OpenAI service, gpt-35-turbo and dall-e-3 models) using the Azure Developer CLI. Only the regions that support the Azure OpenAI service with the models will be displayed. 
	```bash
	azd up
	```

	> 💡 If you already have an Azure OpenAI service available, you can skip the deployment and use hardcoded value in the `program.cs`.
	

## Trying the samples

Now that your Azure OpenAI Service is deployed, It's time to select one of our sample to experiment different scenarios:

- [Hike Benefits Summary](01-HikeBenefitsSummary/README.md): Summarize a long text to a few words.
- [Hiker AI](02-HikerAI/README.md): Chat with the AI and get hike recommendation.
- [Chatting About my Previous Hikes](03-ChattingAboutMyHikes/README.md): Chat with the AI about your previous hikes.
- [AI and Native .NET](04-AiAndNative/README.md): TBD
- [Hike Images](05-HikeImages/README.md): Generate postal card images to invite your friends for a hike.


## Clean up resources

When you are done experimenting with the samples, you can delete the Azure resources created using the Azure Developer CLI.

```bash
azd down
```


## Troubleshooting

On Windows, you may get an error message: "*postprovision.ps1 is not digitally signed. The script will not execute on the system*" after the deployment. This is cause by the script "postprovision" being executed locally after the deployment to create .NET secret that will be used in the application. To avoid this error, execute the command `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`. And re-run the `azd up` command.
