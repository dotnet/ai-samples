# Exercise - Add Logging

1. Open the project you have created in [03 Add Plugin (Function Call)](./03%20Add%20Plugin%20(Function%20Call).md) in VS Code or Visual Studio.

1. Install Extensions Logging nuget package

      ```shell
      dotnet add package Microsoft.Extensions.Logging
      ```

1. Install Extensions Logging console nuget package

      ```shell
      dotnet add package Microsoft.Extensions.Logging.Console
      ```

1. Add the following using statments at the top of `Program.cs` file.

      ```csharp
      using Microsoft.Extensions.DependencyInjection;
      using Microsoft.Extensions.Logging;
      ```

1. Add logging services to the builder before initializing the kernel

      ```csharp
      // Add logging services to the builder
      builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
      ```

1. Run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "Hello" " you will get something similar output as shown below

      ```console
      Q: Hello
      trce: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            ChatHistory: [{"Role":{"Label":"user"},"Items":[{"$type":"TextContent","Text":"Hello"}]}], Settings: {"temperature":1,"top_p":1,"presence_penalty":0,"frequency_penalty":0,"max_tokens":null,"stop_sequences":null,"results_per_prompt":1,"seed":null,"response_format":null,"chat_system_prompt":null,"token_selection_biases":null,"ToolCallBehavior":null,"User":null,"logprobs":null,"top_logprobs":null,"model_id":null}
      info: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            Prompt tokens: 8. Completion tokens: 9. Total tokens: 17.
      Hello! How can I help you today?
      Q:
      ```

> **Note:**  From the output on the console, notice the log information that provided detailed information about our model settings.

## Complete sample project

View the completed sample in the [04 Add Logging)](../../04%20-%20Add%20Logging/) project.

### Next unit: Exercise - Add Plugin - Bing Search

[Continue](./05%20Add%20Plugin%20(Bing%20Search).md)