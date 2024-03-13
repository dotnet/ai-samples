// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoGen.OpenAI;
using FormAssistant;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string openAIEndpoint = config["AZURE_OPENAI_ENDPOINT"] ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new Exception("Please set AZURE_OPENAI_ENDPOINT environment variable.");
string openAIDeploymentName = config["AZURE_OPENAI_GPT_NAME"] ?? throw new Exception("Please set AZURE_OPENAI_GPT_NAME environment variable.");
string openAiKey = config["AZURE_OPENAI_KEY"] ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new Exception("Please set AZURE_OPENAI_API_KEY environment variable.");

AzureOpenAIConfig gptConfig = new AzureOpenAIConfig(endpoint: openAIEndpoint, deploymentName: openAIDeploymentName, apiKey: openAiKey);
var applicationAgent = AgentFactory.CreateFormAgent(gptConfig);
var assistantAgent = AgentFactory.CreateAssistantAgent(gptConfig);
var user = AgentFactory.CreateUserAgent(gptConfig);

var userToApplicationTransition = Transition.Create(user, applicationAgent);
var applicationToAssistantTransition = Transition.Create(applicationAgent, assistantAgent);
var assistantToUserTransition = Transition.Create(assistantAgent, user);

var workflow = new Workflow(
    [
        userToApplicationTransition,
        applicationToAssistantTransition,
        assistantToUserTransition,
    ]);

var groupChat = new GroupChat(
    members: [user, applicationAgent, assistantAgent],
    workflow: workflow);

var groupChatManager = new GroupChatManager(groupChat);
var initialMessage = await assistantAgent.SendAsync("Generate a greeting meesage for user and start the conversation by asking what's their name.");

var chatHistory = await user.SendAsync(groupChatManager, [initialMessage], maxRound: 30);

var lastMessage = chatHistory.Last();
Console.WriteLine(lastMessage.GetContent());
