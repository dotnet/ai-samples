# Hike Summary

This sample demonstrates how to use the Azure OpenAI with a `gpt-35-turbo` model, from a simple .NET 8.0 console application. Use the AI model to summarize a page of text to a few words. It consists of a simple console application, running locally, that will read the file `benefits.md` and send request to an Azure OpenAI service deployed in your Azure subscription to summarize it. 

Everything will be deployed automatically using the Azure Developer CLI.

## Getting Started

1. If it's not already done, follow the [Getting-started](../README.md#getting-started) to deploy the Azure OpenAI service and the `gpt-35-turbo` model.

2. From a terminal or command prompt, navigate to the `01-HikeSummary` directory.
   
3. It's now time to try the console application. Depending on your Azure subscription it's possible that a few (~5) minutes more minute are required before the model deployed in Azure OpenAI get available. If you get an error message about this, wait a few minutes and try again.
	```bash
	dotnet run
	```
4. (Optional) Try to change the content of the file or the length of the summary to see the differences in the responses.
   
5. Once you are done delete the Azure recourse with the following command.
	```bash
	azd down
	```
