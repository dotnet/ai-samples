# Exercise - Add plugin (Bing Search)

This enables the model to search on web to respond for the user request.

## Create the console application

 Run the following command on `PowerShell` to create a new .NET application named **05 - Add Plugin (Bing Search)**.

  ```shell
  dotnet new console -n 05 - Add Plugin (Bing Search)
  ```

2. Switch to the newly created `05 - Add Plugin (Bing Search)` directory.

```shell
cd 05 - Add Plugin (Bing Search)
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

6. Install SemanticKernel.Plugins.Web" nuget package

```shell
dotnet add packageMicrosoft.SemanticKernel.Plugins.Web
```

7. Open the project in VS Code or Visual Studio.

8. In the Program.cs file, delete all the existing code.

9. Add the following using statments at the top of `Program.cs` file.

```Csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
```

10. Add the the following code : Model name and create builder

```csharp
var openAIChatCompletionModelName = "gpt-4-turbo"; // this could be other models like "gpt-4o".

var builder = Kernel.CreateBuilder();
```

11. Initialize the kernel 

```csharp
var kernel = builder
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();
```

12. Import Plugin from WebSearchEnginePlugin object by creating bing connector using Bing API key.

```csharp
kernel.ImportPluginFromObject(new WebSearchEnginePlugin(
    new BingConnector(Environment.GetEnvironmentVariable("BING_API_KEY"))));
```

13. Execution setting

```csharp
var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };// Set the settings for the chat completion service.
```

14. Finally add the common badic chat section

```csharp

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

15.  Run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "What are the major Microsoft announcements in Build 2024?"
you will get something similar output as shown below

```console

1. **Windows AI Features**: Microsoft announced new AI features for Windows, enhancing its capabilities for AI-enabled applications and integrating more deeply with cloud services.

2. **Microsoft Copilot Expansion**: Updates to Microsoft's AI chatbot Copilot, including new capabilities and the introduction of Copilot+ PCs.

3. **Developer Tools**: Novel tools for developers, making it easier to innovate with cost-efficient and user-friendly cloud solutions.

4. **.NET 9 Preview 4**: Announcement of new features in .NET 9 from the .NET sessions.

5. **Microsoft Teams Tools**: New tools and updates for Microsoft Teams to improve collaboration and productivity.

6. **Surface Products**: Announcements of the new Surface Pro 11 and Surface Laptop 7.

7. **Real-Time Intelligence**: Enhancements in AI applications for businesses, allowing for in-the-moment decision making and efficient data organization at ingestion.

8. **GPT-4o**: General availability for using GPT-4o with Azure credits, aimed at aiding developers to build and deploy applications more efficiently.

9. **Rewind Feature**: Introduction of Rewind, a new feature to improve the search functionality on Windows PCs, making it as efficient as web searches.

These announcements span various areas from AI advancements and new hardware to enhanced developer tools and software capabilities.
```

### Next unit: Exercise - Modify Kernel Behavior with Dependency Injection

[Continue](./06%20Modifying%20Kernel%20Behavior%20with%20Dependency%20Injection.md)