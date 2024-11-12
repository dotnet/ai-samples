# Hiking Benefits Summary

This sample demonstrates how to use OpenAI with the `gpt-3.5-turbo` model, from a .NET 8.0 console application. Use the AI model to summarize a page of text to a few words. It consists of a console application, running locally, that will read the file `benefits.md` and send a request to the OpenAI service to summarize it. 

## Getting your OpenAI Key

If it's not already done, get an [API key from OpenAI](https://platform.openai.com/docs/quickstart/account-setup) so you can run this sample.

## Trying Hiking Benefits

1. From a terminal or command prompt, navigate to the `01-HikeBenefitsSummary` directory.

1. Run the following commands to configure your OpenAI API key to run the sample, using the key you previously got from OpenAI.
	```bash
	dotnet user-secrets init
	dotnet user-secrets set OpenAIKey <your-openai-key>
	```
   
1. It's now time to try the console application.
	```bash
	dotnet run
	```

1. (Optional) Try to change the content of the file or the length of the summary to see the differences in the responses.

1. (Optional) Try another sample from the [Getting-started: Trying the samples](../README.md#trying-the-samples) to experiment with different scenarios.
