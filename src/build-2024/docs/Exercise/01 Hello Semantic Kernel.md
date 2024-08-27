# Exercise - Hello Semantic Kernel
<!--TODo: Time to complelet the exercise should be included-->
Now we have an understanding of semantic kernel library and  chat completions,let's create a basic console application that uses them.

## Create the console application

1. Run the following command on `PowerShell` to create a new .NET application named **HelloBuild** but you can substitute the name of your choice.

    ```shell
    dotnet new console -n HelloBuild
    ```

1. Switch to the newly created `HelloBuild` directory.

    ```shell
    cd HelloBuild
    ```

1. Install Semantic Kernel nuget package

    ```shell
    dotnet add package Microsoft.SemanticKernel
    ```

1. Open the project in VS Code or Visual Studio.

1. In the Program.cs file, delete all the existing code.

1. Add `using Microsoft.SemanticKernel;` to the top of Program.cs.

1. Add a model to use for chat completions [Learn more about OpenAI model versions and their capabilities](https://platform.openai.com/docs/models/overview).

    ```csharp
    string openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4o".
    ```

1. Initialize the kernel and add the OpenAI chat completion service to it

      ```csharp
      var kernel = Kernel.CreateBuilder()
          .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
          .Build();
      ```

1. Receive the user's request and send it to the kernel to obtain a response from the LLM.

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

1. Let's see what we have so far, you can run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "Hi my name is Alice" and a follow-up question "what is my name?" your output may vary, but it will be similar to what is shown below.

      ```console
      Q: Hi my name is Alice
      Hello Alice, pleased to meet you! How can I assist you today?
      Q: What is my name?
      I'm sorry, I cannot provide your name as I do not have that information. If you would like me to refer to you by a specific name during our conversation, please let me know.
      Q:
      ```

## Complete sample project

You can view the completed project in the [01 Hello Semantic Kernel](../../01%20-%20Hello%20Semantic%20Kernel) folder.

## Note

  This application is stateless, meaning the AI does not remember any previous interactions. Each input is treated independently.

### Next unit: Exercise - Add Chat History

[Continue](./02%20Add%20Chat%20History.md)
