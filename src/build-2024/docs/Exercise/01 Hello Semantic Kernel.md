# Exercise - Hello Semantic Kernel
<!--TODo: Time to complelet the exercise should be included-->
Now we have an understanding of semantick kerenel libraray and  chat completions,let's create a basic console application that uses them.

## Create the console application

1. Run the following command on `PowerShell` to create a new .NET application named **01 - Hello Semantic 
  Kernel**.

    ```shell
    dotnet new console -n 01 - Hello Semantic Kernel
    ```

1. Switch to the newly created `01 - Hello Semantic Kernel` directory.

    ```shell
    cd 01 - Hello Semantic Kernel
    ```

1. Install Semantic Kernel nuget package

    ```shell
    dotnet add package Microsoft.SemanticKernel
    ```

1. Open the project in VS Code or Visual Studio.

1. In the Program.cs file, delete all the existing code.

1. Add `using Microsoft.SemanticKernel;` to the top of Program.cs.

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

1. Receive user request and  send the request to the kernel to obtain response from LLM.

    ```Csharp
    // Basic chat
    // This is zero memory or stateless chat. The AI will not remember anything from the previous messages.
    while (true)
    {
        Console.Write("Q: ");
        Console.WriteLine(await kernel.InvokePromptAsync(Console.ReadLine()!));
    }
    ```

    - This loop continuously prompts the user for input, sends the input to the OpenAI model, and prints the AI's response.

1. Let's see what we have so far, you can run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "Hi my name is Alice" and a follow-up question "what is my name?" you will get something similar output as shown below on console.

      ```console
      Q: Hi my name is Alice
      Hello Alice, pleased to meet you! How can I assist you today?
      Q: What is my name?
      I'm sorry, I cannot provide your name as I do not have that information. If you would like me to refer to you by a specific name during our conversation, please let me know.
      Q:
      ```

## Note

  This application is stateless, meaning the AI does not remember any previous interactions. Each input is treated independently.

### Next unit: Exercise - Add Chat History

[Continue](./02%20Add%20Chat%20History.md)
