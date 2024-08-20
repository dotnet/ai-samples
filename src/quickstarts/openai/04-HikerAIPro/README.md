# HikerAI Pro

This sample demonstrates how to use OpenAI with the `gpt-3.5-turbo` model to use tool calling (function calling) to invoke a local .NET method, from a .NET 8.0 console application. Get a hiking recommendation, based on the weather condition from the AI model. It consists of a console application, running locally, that will send request to the OpenAI Service.

## Getting your OpenAI Key

If it's not already done, get an [API key from OpenAI](https://platform.openai.com/docs/quickstart/account-setup) so you can run this sample.

## Trying HikerAI Pro

1. From a terminal or command prompt, navigate to the `04-HikerAIPro` directory.

1. Run the following commands to configure your OpenAI API key to run the sample, using the key you previously got from OpenAI.
	```bash
	dotnet user-secrets init
	dotnet user-secrets set OpenAIKey <your-openai-key>
	```

1. It's now time to try the console application.
	```bash
	dotnet run
	```

1. (Optional) Try another sample from the [Getting-started: Trying the samples](../README.md#trying-the-samples) to experiment with different scenarios.