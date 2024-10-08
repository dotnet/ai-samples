# Microsoft.Extensions.AI - Abstraction Examples

This project contains a set of samples with reference implementations for the interfaces included in the [Microsoft.Extensions.AI.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.AI.Abstractions/) and [Microsoft.Extensions.AI](https://www.nuget.org/packages/Microsoft.Extensions.AI/) NuGet packages.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://visualstudio.microsoft.com/downloads/)

## Quick Start

### Visual Studio

1. Open the *AbstractionImplementationExamples.sln* solution
1. Set *AbstractionImplementationExamples* as the startup project.
1. Press <kbd>F5</kbd>.

### Visual Studio Code

1. Open your terminal
1. Navigate to the *AbstractionImplementationExamples* project directory
1. Run the applicaton using `dotnet run`

    ```dotnetcli
    dotnet run
    ```

## Test your application

1. When the application starts, select **Choose sample**.
1. Select one of the samples from the dropdown to run it. 
1. After the selected sample runs, you can choose to run another sample or select **Quit** to stop the application.

## Reference implementations

The following are a set of reference implementations of the interfaces provided by Microsoft.Extensions.AI.Abstractions package.

| Implementation | Description |
| --- | --- | 
| [SampleChatClient](./SampleChatClient.cs) | Sample implementation of `IChatClient` interface. |
| [SampleEmbeddingGenerator](./SampleEmbeddingGenerator.cs) | Sample implementation of `IEmbeddingGenerator` interface. |
| [LoggingChatClient](./LoggingChatClient.cs) | Sample implementation of `DelegatingChatClient` that extends `IChatClient` with logging functionality. | 
| [LoggingEmbeddingGenerator](./LoggingEmbeddingGenerator.cs) | Sample implementation of `DelegatingEmbeddingGenerator` that extends `IEmbeddingGenerator` with logging functionality. | 

## Examples

| Example | Description |
| --- | --- |
| [Chat](./Chat.cs) | Use `IChatClient` to send and receive chat messages |
| [Chat + Conversation History](./ConversationHistory.cs) | Use `IChatClient` alongside conversation history to send and receive chat messages |  
| [Streaming](./Streaming.cs) | Use `IChatClient` to send and receive a stream of chat messages | 
| [Caching](./Caching.cs) | Use prompt caching middleware |
| [OpenTelemetry](./OpenTelemetry.cs) | Use OpenTelemetry middleware | 
| [Tool Calling](./ToolCalling.cs) | Use tool calling middleware | 
| [Middleware](./Middleware.cs) | Use prompt caching, OpenTelemetry and tool calling middleware | 
| [Dependency Injection](./DependencyInjection.cs) | Register an `IChatClient` and middleware using Dependency Injection |
| [Text Embedding](./TextEmbedding.cs) | Use text embedding generator |
| [Text Embedding + Caching](./TextEmbeddingCaching.cs) | Use text embedding generator with caching middleware | 
| [Logging Chat Client](./Logging.cs) | Use `LoggingChatClient` implentation | 
| [Logging Embedding Generator](./Logging.cs) | Use `LoggingEmbeddingGenerator` implementation | 