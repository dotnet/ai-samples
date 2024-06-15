# Exercise - Add Logging

## Create the console application

1. Run the following command on `PowerShell` to create a new .NET application named **04 - Add Logging**.

  ```shell
  dotnet new console -n 04 - Add Logging
  ```

2. Switch to the newly created `04 - Add Logging` directory.

```shell
cd 02 - 04 - Add Logging
```

3. Install Semantic Kernel nuget package

```shell
dotnet add package Microsoft.SemanticKernel
```

4. Install Extensions Logging nuget package

```shell
dotnet add package Microsoft.Extensions.Logging
```

5. Install Extensions Logging console nuget package

```shell
dotnet add package Microsoft.Extensions.Logging.Console
```

6. Open the project in VS Code or Visual Studio.

7. In the Program.cs file, delete all the existing code.

8. Add the following using statments at the top of `Program.cs` file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
```
9. Add the  the following code : Model name and create builder

```csharp
var openAIChatCompletionModelName = "gpt-4-turbo"; // this could be other models like "gpt-4o".

var builder = Kernel.CreateBuilder();
```

10.  Add logging services to the builder

```csharp
// Add logging services to the builder
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
```
11. Initialize the kernel 

```csharp
var kernel = builder
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();
```

The rest of the code is similar to [02 - Add Chat history](./02%20Add%20Chat%20History.md).

12. Run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "Hello" " you will get something similar output as shown below

```console
Q: Hello
trce: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
      ChatHistory: [{"Role":{"Label":"user"},"Items":[{"$type":"TextContent","Text":"Hello"}]}], Settings: {"temperature":1,"top_p":1,"presence_penalty":0,"frequency_penalty":0,"max_tokens":null,"stop_sequences":null,"results_per_prompt":1,"seed":null,"response_format":null,"chat_system_prompt":null,"token_selection_biases":null,"ToolCallBehavior":null,"User":null,"logprobs":null,"top_logprobs":null,"model_id":null}
info: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
      Prompt tokens: 8. Completion tokens: 9. Total tokens: 17.
Hello! How can I help you today?
Q:
```

> **Note:**  From the outout on console notice the log  information that provided detailed information about our model settings. 


### Next unit: Exercise - Add Plugin - Bing Search

[Continue](./05%20Add%20Plugin%20(Bing%20Search).md)