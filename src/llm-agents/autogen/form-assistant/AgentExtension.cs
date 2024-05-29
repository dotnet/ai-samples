// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FormAssistant;

internal static class AgentExtension
{
    public static MiddlewareAgent<TAgent> RegisterSpectreConsoleOutput<TAgent>(this TAgent agent)
        where TAgent : IAgent
    {
        var middleware = new SpectreOutputMiddleware();
        return agent.RegisterMiddleware(middleware);
    }
}
