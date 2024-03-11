// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// == Retrieve the local secrets saved during the Azure deployment ==========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"];
string openAiKey = config["AZURE_OPENAI_KEY"];
string openAIDalleName = config["AZURE_OPENAI_DALLE_NAME"];
// == If you skipped the deployment because you already have an Azure OpenAI available,
// == edit the previous lines to use hardcoded values.
// == ex: string openAIEndpoint = "https://cog-demo123.openai.azure.com/";

// == Create the Azure Open AI Text to Image Service ==========
#pragma warning disable SKEXP0012 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
AzureOpenAITextToImageService textToImageService = new(
            deploymentName: openAIDalleName,
            endpoint: openAIEndpoint,
            apiKey: openAiKey,
            null);
#pragma warning restore SKEXP0012 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


// == Define the image ==========
string imagePrompt = """
A postal card with an happy hiker waving, there a beautiful mountain in the background.
There is a trail visible in the foreground. 
The postal card has text in red saying: 'You are invited for a hike!'
""";

#pragma warning disable SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var imageUrl = await textToImageService.GenerateImageAsync(imagePrompt.Trim(), 1024, 1024);
#pragma warning restore SKEXP0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine($"\n\nThe generated image is ready at:\n {imageUrl}");
