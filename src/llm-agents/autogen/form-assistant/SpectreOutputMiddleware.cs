// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace FormAssistant;

/// <summary>
/// Pretty print the message using spectre console.
/// </summary>
internal class SpectreOutputMiddleware : IMiddleware
{
    public string? Name => nameof(SpectreOutputMiddleware);

    public async Task<IMessage> InvokeAsync(MiddlewareContext context, IAgent agent, CancellationToken cancellationToken = default)
    {
        if (agent is IStreamingAgent streamingAgent)
        {
            var table = new Table();
            return await AnsiConsole.Live(table)
                .StartAsync(async ctx =>
                {
                    IMessage? latestReply = default;
                    var streamingReplyTask = await streamingAgent.GenerateStreamingReplyAsync(context.Messages, context.Options, cancellationToken);
                    await foreach (var reply in streamingReplyTask)
                    {
                        if (reply is TextMessageUpdate update)
                        {
                            if (latestReply is null)
                            {
                                latestReply = new TextMessage(update);
                            }
                            else
                            {
                                (latestReply as TextMessage)!.Update(update);
                            }
                        }
                        else if (reply is ToolCallMessageUpdate toolCallUpdate)
                        {
                            if (latestReply is null)
                            {
                                latestReply = new ToolCallMessage(toolCallUpdate);
                            }
                            else
                            {
                                (latestReply as ToolCallMessage)!.Update(toolCallUpdate);
                            }
                        }
                        else if (reply is IMessage)
                        {
                            latestReply = reply as IMessage;
                        }

                        if (latestReply is not null)
                        {
                            table = CreateTableFromMessage(latestReply);
                            ctx.UpdateTarget(table);
                        }
                    }

                    return latestReply;
                }) ?? throw new InvalidOperationException("The latest reply should not be null.");
        }
        else
        {
            var reply = await agent.GenerateReplyAsync(context.Messages, context.Options, cancellationToken);
            var table = CreateTableFromMessage(reply);
            AnsiConsole.Write(table);

            return reply;
        }
    }

    private Table CreateTableFromMessage(IStreamingMessage message)
    {
        var table = new Table();
        table.AddColumn("header");
        table.HideHeaders();
        table.Border = TableBorder.None;
        if (message.From is string from)
        {
            table.AddRow($"[bold yellow]from[/] [italic blue]{from}[/]");
        }

        if (message is TextMessage textMessage)
        {
            // replace [ with [[ to avoid markdown parsing
            var content = textMessage.Content ?? string.Empty;
            content = content.Replace("[", "[[");

            // replace ] with ]] to avoid markdown parsing
            content = content.Replace("]", "]]");
            table.AddRow(content);
        }
        else if (message is ToolCallMessage toolCallMessage)
        {
            foreach (var toolCall in toolCallMessage.ToolCalls)
            {
                var functionName = toolCall.FunctionName;
                var functionArguments = toolCall.FunctionArguments;
                table.AddRow($"[bold yellow]function[/] [italic blue]{functionName}[/]");
                table.AddRow($"[bold yellow]arguments[/] [italic blue]{functionArguments}[/]");
            }
        }
        else if (message is AggregateMessage<ToolCallMessage, ToolCallResultMessage> aggregateMessage)
        {
            var toolCallResultMessage = aggregateMessage.Message2;

            foreach (var toolCall in toolCallResultMessage.ToolCalls)
            {
                var functionName = toolCall.FunctionName;
                var result = toolCall.Result;
                table.AddRow($"[bold yellow]result[/] of function [italic blue]{functionName}[/]");
                table.AddRow($"[italic grey]{result}[/]");
            }
        }

        return table;
    }
}
