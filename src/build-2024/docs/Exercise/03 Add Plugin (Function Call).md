# Exercise - Add Plugin (Function Call)

This project demonstrates how to extend OpenAI's Semantic Kernel capabilities by incorporating additional services like plugins.

##  Create the console application

1. Run the following command on `PowerShell` to create a new .NET application named **03 - Add Plugin (Function Call)**.

    ```shell
    dotnet new console -n 03 - Add Plugin (Function Call)
    ```

1. Switch to the newly created `03 - Add Plugin (Function Call)` directory.

    ```shell
    cd 03 - Add Plugin (Function Call)
    ```

1. Install Semantic Kernel nuget package

    ```shell
    dotnet add package Microsoft.SemanticKernel
    ```

1. Open the project in VS Code or Visual Studio.

1. In the Program.cs file, delete all the existing code.

1. Add the following using statments to the top of `Program.cs` file.

    ```csharp
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    ```

1. Add a compilation model name. To learn more about OpenAI model versions and their capability refer [this](https://platform.openai.com/docs/models/overview).

    ```csharp
    string openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4o".
    ```

1. Initializing the kernel and add OpenAI chat compilation service to it.

    ```csharp
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
        .Build();
    ```

1. Define a class named `DemographicInfo` with `GetAge` function at the bottom of `Program.cs`. The GetAge function is also decorated with KernelFunction to mark it as a kernel function.

    ```csharp
    class DemographicInfo
    {
        [KernelFunction]
        public int GetAge(string name)
        {
            return name switch
            {
                "Alice" => 25,
                "Bob" => 30,
                _ => 0
            };
        }
    }
    ```

1. Add the following next to kerenel intilization to import a plugin to the kernel from type `DemographicInfo`.

    ```csharp
    // Import the DemographicInfo class to the kernel, so it can be used in the chat completion service.
    // this plugin could be from other options such as functions, prompts directory, etc.
    kernel.ImportPluginFromType<DemographicInfo>();
    ```

1. Add the following function calling  behavior setting. The setting is configured to call the method GetAge automatically when the user requests the age of the person with the provided name.

    ```csharp
    var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };// Set the settings for the chat completion service.
    ```

1. Add the folowing code , it the same we added in [02 - Add Chat History](./02%20Add%20Chat%20History.md)  excpt ` var response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);`  in whcih we passed `settings` and `kernel` to `chatService.GetChatMessageContentAsync`

    ```csharp
    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    ChatHistory chatHistory = [];

    // Basic chat
    while (true)
    {
        Console.Write("Q: ");
        chatHistory.AddUserMessage(Console.ReadLine());// Add user message to chat history, then it can be use to get more context for the next chat response

        var response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);// Get chat response based on chat history

        Console.WriteLine(response);
        chatHistory.Add(response);// Add chat response to chat history, hence it can be use to get more context for the next chat response
    }
    ```
1. Run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "Hi my name is Alice" and a follow-up question "How old am I?" you will get something similar output as shown

    ```console
    Q: Hi my name is Alice
    Hello Alice! How can I assist you today? Also, if you're wondering, I found that you might be around 25 years old. Is there anything specific you'd like to talk about or do?
    Q: How old am I?
    You are 25 years old. If you have any other questions or need assistance with something, feel free to ask!
    Q:
    ```

### Next unit: Exercise - Add Logging

[Continue](./04%20Add%20Logging.md)