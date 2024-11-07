# Generate Hiking Images

This sample demonstrates how to use the Azure OpenAI with a `dall-e-3` model, from a simple .NET 8.0 console application. Use the AI model to generate postal card and invite your friends for a hike! It consists of a simple console application, running locally, that send request to an Azure OpenAI service deployed in your Azure subscription to generate image based on a prompt. 

Everything will be deployed automatically using the Azure Developer CLI.


## Deploying the Azure resources

If it's not already done, follow the [Getting-started: Deploying the Azure resources](../../README.md#deploying-the-azure-resources) to deploy the Azure OpenAI service with the models.


## Trying Chatting About My Previous Hikes 

1. From a terminal or command prompt, navigate to the `05-HikeImages` directory.
   
2. It's now time to try the console application. Depending on your Azure subscription it's possible that a few more minutes are required before the model deployed in Azure OpenAI get available. If you get an error message about this, wait a few (~5) minutes and try again.
	```bash
	dotnet run
	```

3. (Optional) Try edit the `imagePrompt` variable in the `Program.cs`, try different prompts to personalize the images generated.

4. (Optional) Try another sample from the [Getting-started: Trying the samples](../README.md#trying-the-samples) to experiment with different scenarios.


## Clean up resources

Once you are done experimenting with the samples, follow the instructions from the [Getting-started: Clean up resources](../../README.md#clean-up-resources) to delete the Azure resources created using the Azure Developer CLI.