# Hiking Benefits Summary

This sample demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` model, from a .NET 8.0 console application. Use the AI model to summarize a page of text to a few words. It consists of a console application, running locally, that will read the file `benefits.md` and send request to an Azure OpenAI Service deployed in your Azure subscription to summarize it. 

Everything will be deployed automatically using the Azure Developer CLI.


## Deploying the Azure resources

If it's not already done, follow the [Getting-started: Deploying the Azure resources](../../README.md#deploying-the-azure-resources) to deploy the Azure OpenAI Service with the models.


## Trying Hiking Benefits

1. From a terminal or command prompt, navigate to the `01-HikeBenefitsSummary` directory.
   
2. It's now time to try the console application. Depending on your Azure subscription it's possible that a few more minutes are required before the model deployed in Azure OpenAI is available. If you get an error message about this, wait a few (~5) minutes and try again.
	```bash
	dotnet run
	```

3. (Optional) Try to change the content of the file or the length of the summary to see the differences in the responses.

4. (Optional) Try another sample from the [Getting-started: Trying the samples](../../README.md#trying-the-samples) to experiment different scenarios.


## Clean up resources

Once you are done experimenting with the samples, follow the instructions from the [Getting-started: Clean up resources](../../README.md#clean-up-resources) to delete the Azure resources created using the Azure Developer CLI.