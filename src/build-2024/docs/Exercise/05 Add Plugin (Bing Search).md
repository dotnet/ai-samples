# Exercise - Add plugin (Bing Search)

This enables the model to perform web searches in order to respond to user requests.

1. Open the project you have created in [04 Add Logging](./05%20Add%20Plugin%20(Bing%20Search).md) in VS Code or Visual Studio.

1. Install `SemanticKernel.Plugins.Web nuget` package

    ```shell
    dotnet add package Microsoft.SemanticKernel.Plugins.Web --prerelease
    ```

1. Add the following using statments at the top of `Program.cs` file.

    ```Csharp
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    using Microsoft.SemanticKernel.Plugins.Web;
    using Microsoft.SemanticKernel.Plugins.Web.Bing;
     ```

1. Import the plugin from the WebSearchEnginePlugin object by creating a Bing connector using the Bing API key and suppress warning `SKEXP0050`

    ```csharp
    #pragma warning disable SKEXP0050
    kernel.ImportPluginFromObject(new WebSearchEnginePlugin(
        new BingConnector(Environment.GetEnvironmentVariable("BING_API_KEY"))));
    #pragma warning restore SKEXP0050
    ```

1. Run the application by entering `dotnet run` into the terminal. Experiment with a user prompt "What are the major Microsoft announcements in Build 2024?"
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

## Complete sample project

View the completed sample in the [05 Add Plugin (Bing Search)](../../05%20-%20Add%20Plugin%20(Bing%20Search)) project.

### Next unit: Exercise - Modify Kernel Behavior with Dependency Injection

[Continue](./06%20Modifying%20Kernel%20Behavior%20with%20Dependency%20Injection.md)