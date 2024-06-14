# Exercise - Add Plugin (Function Call)

This project demonstrates how to extend OpenAI's Semantic Kernel capabilities by incorporating additional services like plugins.

## Overview

>Most of the code in this project is the same with `02 - Add Chat History` project codes. You can refer its [README.md](../02%20-%20Add%20Chat%20History/README.md).

1. Using statments

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
```

2. Import Plugins :  This enables LLM to call our native function `GetAge` defind in `DemographicInfo`. To learn more about native functions [Creating native functions for AI to call](https://learn.microsoft.com/en-us/semantic-kernel/agents/plugins/using-the-kernelfunction-decorator?tabs=Csharp).

```csharp
// Import the DemographicInfo class to the kernel, so it can be used in the chat completion service.
// this plugin could be from other options such as functions, prompts directory, etc.
kernel.ImportPluginFromType<DemographicInfo>();
var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };// Set the settings for the chat completion service.

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

The setting is configured to call the method GetAge automatically when the user requests the age of the person with the provided name. The GetAge function is also decorated with KernelFunction to mark it as a kernel function.

```csharp
var settings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions }
```

### Next unit: Exercise - Add Logging

[Continue](./04%20Add%20Logging.md)