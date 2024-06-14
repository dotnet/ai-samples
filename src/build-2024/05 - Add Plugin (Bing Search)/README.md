# Exercise - Add plugin (Bing Search)

## Overview

> For full code explnation please refer readme for `02 - Add Chat History`  [README.md](../02%20-%20Add%20Chat%20History/README.md).

- **Prerequisite**
  - [Bing search API key](https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/create-bing-search-service-resource)

- **Using statments**

```Csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
```

- **Import plugin from web search engine:**

This enables the model to search on web to respond for the user request.

```csharp
kernel.ImportPluginFromObject(new WebSearchEnginePlugin(
    new BingConnector(Environment.GetEnvironmentVariable("BING_API_KEY"))));
```

### Next unit: Exercise - Modify Kernel Behavior with Dependency Injection

[Continue](../06%20-%20Modifying%20Kernel%20Behavior%20with%20Dependency%20Injection/README.md)