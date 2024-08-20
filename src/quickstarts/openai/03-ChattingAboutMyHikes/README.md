# Chatting About My Previous Hikes

This sample demonstrates how to use OpenAI with a `gpt-3.5-turbo` model, from a .NET 8.0 console application. Use the AI model to get analytics and information about your previous hikes. It consists of a console application, running locally, that will read the file `hikes.md` and send the request to the OpenAI Service and provide the result in the console. 

## Getting your OpenAI Key

If it's not already done, get an [API key from OpenAI](https://platform.openai.com/docs/quickstart/account-setup) so you can run this sample.

## Trying Chatting About My Previous Hikes 

1. From a terminal or command prompt, navigate to the `03-ChattingAboutMyHikes` directory.

1. Run the following commands to configure your OpenAI API key to run the sample, using the key you previously got from OpenAI.
	```bash
	dotnet user-secrets init
	dotnet user-secrets set OpenAIKey <your-openai-key>
	```
   
2. It's now time to try the console application.
	```bash
	dotnet run
	```

3. (Optional) Try changing the `hikeRequest` variable in the `Program.cs`, asking for different questions (ex: How many times did you hiked when it was raining? How many times did you hiked in 2021? etc.)

4. (Optional) Try another sample from the [Getting-started: Trying the samples](../README.md#trying-the-samples) to experiment with different scenarios.