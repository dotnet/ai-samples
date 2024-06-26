﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Retrieve the local secrets saved during the Azure deployment. If you skipped the deployment
// because you already have an Azure OpenAI available, edit the following lines to use your information,
// e.g. string openAIEndpoint = "https://cog-demo123.openai.azure.com/";
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["AZURE_OPENAI_ENDPOINT"];
string deployment = config["AZURE_OPENAI_DALLE_NAME"];
string key = config["AZURE_OPENAI_KEY"];

// Create the Azure OpeAI Text to Image Service
AzureOpenAITextToImageService textToImageService = new(deployment, endpoint, key, null);

// Generate the image
string imageUrl = await textToImageService.GenerateImageAsync("""
    A postal card with an happy hiker waving and a beautiful mountain in the background.
    There is a trail visible in the foreground.
    The postal card has text in red saying: 'You are invited for a hike!'
    """, 1024, 1024);
Console.WriteLine($"The generated image is ready at:\n{imageUrl}");
