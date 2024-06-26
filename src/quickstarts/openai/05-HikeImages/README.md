# Generate Hiking Images

This sample demonstrates how to use OpenAI with a `dall-e-3` model, from a simple .NET 8.0 console application. Use the AI model to generate postal card and invite your friends for a hike! It consists of a simple console application, running locally, that send request to the OpenAI service to generate image based on a prompt. 

## Getting your OpenAI Key

If it's not already done, get an [API key from OpenAI](https://platform.openai.com/docs/quickstart/account-setup) so you can run this sample.

## Trying Chatting About My Previous Hikes 

1. From a terminal or command prompt, navigate to the `05-HikeImages` directory.

1. Run the following commands to configure your OpenAI API key to run the sample, using the key you previously got from OpenAI.
	```bash
	dotnet user-secrets init
	dotnet user-secrets set OpenAIKey <your-openai-key>
	```
   
1. It's now time to try the console application.
	```bash
	dotnet run
	```

3. (Optional) Try edit the `imagePrompt` variable in the `Program.cs`, try different prompts to personalize the images generated.

4. (Optional) Try another sample from the [Getting-started: Trying the samples](../README.md#trying-the-samples) to experiment with different scenarios.
