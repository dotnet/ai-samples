// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"];
string openAiKey = config["AZURE_OPENAI_KEY"];
string openAIDalleName = config["AZURE_OPENAI_DALLE_NAME"];
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";


// == Creating the AIClient ==========
var endpoint = new Uri(openAIEndpoint);
var credentials = new AzureKeyCredential(openAiKey);
var openAIClient = new OpenAIClient(endpoint, credentials);


// == Define the image ==========
string imagePrompt = """
A postal card with an happy hiker waving, there a beautiful mountain in the background.
There is a trail visible in the foreground. 
The postal card has text in red saying: 'You are invited for a hike!'
""";


Response<ImageGenerations> response = await openAIClient.GetImageGenerationsAsync(
    new ImageGenerationOptions()
    {
        DeploymentName = openAIDalleName,
        Prompt = imagePrompt,
        Size = ImageSize.Size1024x1024,
        Quality = ImageGenerationQuality.Standard
    });

ImageGenerationData generatedImage = response.Value.Data[0];
if (!string.IsNullOrEmpty(generatedImage.RevisedPrompt))
{
    Console.WriteLine($"\n\nInput prompt automatically revised to:\n {generatedImage.RevisedPrompt}");
}
Console.WriteLine($"\n\nThe generated image is ready at:\n {generatedImage.Url.AbsoluteUri}");
