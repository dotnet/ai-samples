# HikerAI Pro

This sample demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` model to use tool calling (function calling) to invoke a local .NET method, from a .NET 8.0 console application. Get a hiking recommendation, based on the weather condition from the AI model. It consists of a console application, running locally, that will send request to an Azure OpenAI Service deployed in your Azure subscription.

## Deploying the Azure resources

If it's not already done, follow the [Getting-started: Deploying the Azure resources](../../README.md#deploying-the-azure-resources) to deploy the Azure OpenAI service with the models.


## Trying HikerAI Pro

1. From a terminal or command prompt, navigate to the `04-HikerAIPro` directory.

2. It's now time to try the console application. Depending on your Azure subscription it's possible that a few more minutes are required before the model deployed in Azure OpenAI get available. If you get an error message about this, wait a few (~5) minutes and try again.
	```bash
	dotnet run
	```

3. (Optional) Try another sample from the [Getting-started: Trying the samples](../README.md#trying-the-samples) to experiment with different scenarios.

## Clean up resources

Once you are done experimenting with the samples, follow the instructions from the [Getting-started: Clean up resources](../../README.md#clean-up-resources) to delete the Azure resources created using the Azure Developer CLI.