This project demonstrates how to extend OpenAI's Semantic Kernel capabilities by incorporating additional services like logging and plugins. Specifically, it shows how to:

1. Add logging services to the Semantic Kernel builder.
```csharp
var builder = Kernel.CreateBuilder();

// Add logging services to the builder
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
```

2. Provide more context to the AI model using plugins.
Plugins provide additional context to the AI model. In this example, we import a `DemographicInfo` class that includes a method to get the age of a person based on the name.
```csharp
// Import the DemographicInfo class to the kernel, so it can be used in the chat completion service.
kernel.ImportPluginFromType<DemographicInfo>();
```
3. Configure the OpenAIPromptExecutionSettings to automatically invoke kernel functions.
```csharp
var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
```