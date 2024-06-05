This project demonstrates how to use the Microsoft Semantic Kernel to build a chat completion service integrated with OpenAI's GPT models and Bing Web Search. The code shows how to set up a kernel with logging, integrate OpenAI chat completion services, and import plugins from objects, such as the Bing Web Search plugin.
### Initializing the Kernel Builder
We create a kernel builder and add logging services to it. The logging level is set to trace, and logs will be output to the console.
```csharp
var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
```
### Adding OpenAI Chat Completion Service
The code adds the OpenAI chat completion service to the kernel. The model used is "gpt-4-turbo", but you can replace it with other models if needed. The OpenAI API key is retrieved from the environment variables.
```csharp
var kernel = builder
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

```
### Importing Bing Web Search Plugin
```csharp#pragma warning disable
kernel.ImportPluginFromObject(new Microsoft.SemanticKernel.Plugins.Web.WebSearchEnginePlugin(
    new BingConnector(Environment.GetEnvironmentVariable("BING_API_KEY"))));
```
### Configuring Chat Settings
We set the chat completion settings, specifically the `ToolCallBehavior` to `AutoInvokeKernelFunctions`, which allows automatic invocation of kernel functions during chat interactions.
```csharp
var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];
```
