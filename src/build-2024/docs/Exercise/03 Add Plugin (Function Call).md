# Exercise - Add Plugin (Function Call)

This project demonstrates how to extend OpenAI's Semantic Kernel functionalities by incorporating additional services like plugins.

1. Open the project you have created in [02 Add Chat History](02%20Add%20Chat%20History.md) in VS Code or Visual Studio.

1. Add the following using statment to the top of `Program.cs` file.

    ```csharp
    using Microsoft.SemanticKernel.Connectors.OpenAI;
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

1. Add the following next to kernel initialization to import a plugin to the kernel from type `DemographicInfo`.

    ```csharp
    // Import the DemographicInfo class to the kernel, so it can be used in the chat completion service.
    // this plugin could be from other options such as functions, prompts directory, etc.
    kernel.ImportPluginFromType<DemographicInfo>();
    ```

1. Add the following function calling behavior setting. The setting is configured to call the method `GetAge` automatically when the user requests the age of the person with the provided name.

    ```csharp
    var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };// Set the settings for the chat completion service.
    ```

1. Modify the while loop  by adding  the following code 

    ```cshrp
    var response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);// Get chat response based on chat history
    ```

1. Run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "My name is Alice. How old Am I?" ,you will get something similar output as shown

    ```console
    Q: My name is Alice. How old am I?
    Alice, you are 25 years old.
    Q: 
    ```

## Complete sample project

View the completed sample in the [03 Add Plugin (Function Call)](../../03%20-%20Add%20Plugin%20(Function%20Call)) project.

### Next unit: Exercise - Add Logging

[Continue](./04%20Add%20Logging.md)