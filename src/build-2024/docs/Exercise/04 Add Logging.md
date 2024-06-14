# Exercise - Add Logging

## Overview

> Most of the code in this project is the same with `02 - Add Chat History` project codes. You can refer its [README.md](../02%20-%20Add%20Chat%20History/README.md). In this readme only specifc codes for this project are explained.

- **Using statments**

```Csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
```

- **Create the Builder**

```csharp
var builder = Kernel.CreateBuilder();
```

- **Add logging to the services of the builder**

This will let us to trace informationat runtime.

```Csharp
// Add logging services to the builder
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
```

### Next unit: Exercise - Add Plugin - Bing Search

[Continue](./05%20Add%20Plugin%20(Bing%20Search).md)