// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoGen.OpenAI;
using Azure.AI.OpenAI;
using FormApplication;

namespace FormAssistant;

internal static class AgentFactory
{
    public static IAgent CreateFormAgent(OpenAIClient client, string deployName)
    {

        var instance = new FillApplicationFunction();
        var openaiMessageConnector = new OpenAIChatRequestMessageConnector();
        var functionCallConnector = new FunctionCallMiddleware(
            functions: [instance.SaveProgressFunctionContract],
            functionMap: new Dictionary<string, Func<string, Task<string>>>
            {
                { instance.SaveProgressFunctionContract.Name!, instance.SaveProgressWrapper },
            });

        var chatAgent = new OpenAIChatAgent(
            openAIClient: client,
            name: "application",
            modelName: deployName,
            systemMessage: """
            You are a helpful form assistant to help user during the application process.
            You save the progress once a piece of information is provided.
            """)
            .RegisterStreamingMiddleware(openaiMessageConnector)
            .RegisterMiddleware(openaiMessageConnector)
            .RegisterMiddleware(functionCallConnector)
            .RegisterStreamingMiddleware(functionCallConnector)
            .RegisterSpectreConsoleOutput()
            .RegisterMiddleware(async (msgs, option, agent, ct) =>
            {
                var lastUserMessage = msgs.Last() ?? throw new Exception("No user message found.");
                var prompt = $"""
                Save progress according to the most recent information provided by user.

                ```user
                {lastUserMessage.GetContent()}
                ```
                """;

                return await agent.GenerateReplyAsync([lastUserMessage], option, ct);

            });

        return chatAgent;
    }

    public static IAgent CreateAssistantAgent(OpenAIClient openaiClient, string deployName)
    {
        var openaiMessageConnector = new OpenAIChatRequestMessageConnector();

        var chatAgent = new OpenAIChatAgent(
            openAIClient: openaiClient,
            name: "assistant",
            modelName: deployName,
            systemMessage: """You create polite prompt to ask user provide missing information""")
            .RegisterStreamingMiddleware(openaiMessageConnector)
            .RegisterMiddleware(openaiMessageConnector)
            .RegisterSpectreConsoleOutput()
            .RegisterMiddleware(async (msgs, option, agent, ct) =>
            {
                var lastReply = msgs.Last() ?? throw new Exception("No reply found.");
                var reply = await agent.GenerateReplyAsync(msgs, option, ct);

                // if application is complete, exit conversation by sending termination message
                if (lastReply.GetContent()?.Contains("Application information is saved to database.") is true)
                {
                    return new TextMessage(Role.Assistant, GroupChatExtension.TERMINATE, from: agent.Name);
                }
                else
                {
                    return reply;
                }
            });

        return chatAgent;
    }

    public static IAgent CreateUserAgent(AzureOpenAIConfig gptConfig)
    {
        var endPoint = gptConfig.Endpoint ?? throw new Exception("Please set AZURE_OPENAI_ENDPOINT environment variable.");
        var apiKey = gptConfig.ApiKey ?? throw new Exception("Please set AZURE_OPENAI_API_KEY environment variable.");
        var openaiClient = new OpenAIClient(new Uri(endPoint), new Azure.AzureKeyCredential(apiKey));

        var openaiMessageConnector = new OpenAIChatRequestMessageConnector();

        var chatAgent = new OpenAIChatAgent(
            openAIClient: openaiClient,
            name: "user",
            modelName: gptConfig.DeploymentName,
            systemMessage: """
            You are a user who is filling an application form. Simply provide the information as requested and answer the questions, don't do anything else.
            
            here's some personal information about you:
            - name: John Doe
            - email: 1234567@gmail.com
            - phone: 123-456-7890
            - address: 1234 Main St, Redmond, WA 98052
            - want to receive update? true
            """)
            .RegisterStreamingMiddleware(openaiMessageConnector)
            .RegisterMiddleware(openaiMessageConnector)
            .RegisterSpectreConsoleOutput();

        return chatAgent;
    }
}
