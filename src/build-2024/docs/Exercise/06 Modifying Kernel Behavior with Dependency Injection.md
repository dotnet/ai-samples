# Exercise - Modifying Kernel Behavior with Dependency Injection

1. Open the project you have created in [04 Add Logging](./05%20Add%20Plugin%20(Bing%20Search).md).md) in VS Code or Visual Studio.

1. Install Extensions.Compliance.Redaction nuget package

      ```shell
      dotnet add package Microsoft.Extensions.Compliance.Redaction
      ```

1. Install Extensions.Http nuget package

      ```shell
      dotnet add package Microsoft.Extensions.Http
      ```

1. Install Extensions.Http.Resilience nuget package

      ```shell
      dotnet add package Microsoft.Extensions.Http.Resilience
      ```

1. Add the following using statment at the top of `Program.cs` file.

      ```cshap
      using Microsoft.Extensions.Http;
      ```

1. Add the following code after `builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));`
 which modifies the kernel behavior by injecting services into it. For this sample we're using experimental features, so we add a `#pragma` statement to suppress warnings about using them.

      ```csharp
      builder.Services.ConfigureHttpClientDefaults(b =>
      {
      b.AddStandardResilienceHandler();
      b.RedactLoggedHeaders(["Authorization"]);
      });
      builder.Services.AddRedaction();

      // injecting the permission filter to the kernel.
      #pragma warning disable SKEXP0001,SKEXP0050
      builder.Services.AddSingleton<IFunctionInvocationFilter, PermissionFilter>();
      ```

1. Add the following code at the end of `Program.cs` to Implment  `IFunctionInvocationFilter`

      ```csharp
      class PermissionFilter : IFunctionInvocationFilter
      {
      public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
      {
            Console.WriteLine($"Allow {context.Function.Name}?");
            if (Console.ReadLine() == "y")
            {
                  await next(context);
            }
            else
            {
                  throw new Exception("Permission denied");
            }
      }
      }
      ```

 1. Run the application by typing `dotnet run` in the terminal. Test it with the user prompt, ‘What are the major Microsoft announcements in Build 2024?’ If prompted to allow search, enter 'y'. You will see an output similar to shown below.

      ```console
      trce: Microsoft.SemanticKernel.Plugins.Web.WebSearchEnginePlugin[0]
            Created KernelFunction 'Search' for 'SearchAsync'
      trce: Microsoft.SemanticKernel.Plugins.Web.WebSearchEnginePlugin[0]
            Created KernelFunction 'GetSearchResults' for 'GetSearchResultsAsync'
      trce: Microsoft.SemanticKernel.Plugins.Web.WebSearchEnginePlugin[0]
            Created plugin WebSearchEnginePlugin with 2 [KernelFunction] methods out of 8 methods found.
      Q: What are the major Microsoft announcements in Build 2024?
      trce: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            ChatHistory: [{"Role":{"Label":"user"},"Items":[{"$type":"TextContent","Text":"What are the major Microsoft announcements in Build 2024?"}]}], Settings: {"temperature":1,"top_p":1,"presence_penalty":0,"frequency_penalty":0,"max_tokens":null,"stop_sequences":null,"results_per_prompt":1,"seed":null,"response_format":null,"chat_system_prompt":null,"token_selection_biases":null,"ToolCallBehavior":{"ToolCallResultSerializerOptions":null},"User":null,"logprobs":null,"top_logprobs":null,"model_id":null}
      info: System.Net.Http.HttpClient.Default.LogicalHandler[100]
            Start processing HTTP request POST https://api.openai.com/v1/chat/completions
      trce: System.Net.Http.HttpClient.Default.LogicalHandler[102]
            Request Headers:
            Accept: application/json
            Semantic-Kernel-Version: 1.13.0.0
            x-ms-client-request-id: b83ec540-7938-41c8-ad48-08cd2bc944c2
            x-ms-return-client-request-id: true
            User-Agent: Semantic-Kernel, azsdk-net-AI.OpenAI/1.0.0-beta.15, (.NET 8.0.5; Microsoft Windows 10.0.22631)
            Authorization: *
            Content-Type: application/json

      dbug: Polly[1]
            Resilience pipeline executing. Source: '-standard/', Operation Key: '(null)'
      info: System.Net.Http.HttpClient.Default.ClientHandler[100]
            Sending HTTP request POST https://api.openai.com/v1/chat/completions
      trce: System.Net.Http.HttpClient.Default.ClientHandler[102]
            Request Headers:
            Accept: application/json
            Semantic-Kernel-Version: 1.13.0.0
            x-ms-client-request-id: b83ec540-7938-41c8-ad48-08cd2bc944c2
            x-ms-return-client-request-id: true
            User-Agent: Semantic-Kernel, azsdk-net-AI.OpenAI/1.0.0-beta.15, (.NET 8.0.5; Microsoft Windows 10.0.22631)
            Authorization: *
            Content-Type: application/json

      info: System.Net.Http.HttpClient.Default.ClientHandler[101]
            Received HTTP response headers after 2160.4263ms - 200
      trce: System.Net.Http.HttpClient.Default.ClientHandler[103]
            Response Headers:
            Date: Mon, 17 Jun 2024 19:29:50 GMT
            Transfer-Encoding: chunked
            Connection: keep-alive
            openai-organization: jakerad
            openai-processing-ms: 1644
            openai-version: 2020-10-01
            Strict-Transport-Security: max-age=15724800; includeSubDomains
            x-ratelimit-limit-requests: 500
            x-ratelimit-limit-tokens: 30000
            x-ratelimit-remaining-requests: 499
            x-ratelimit-remaining-tokens: 29968
            x-ratelimit-reset-requests: 120ms
            x-ratelimit-reset-tokens: 64ms
            X-Request-ID: req_ca74fa8e36b14b8b8abf2c5d7a771662
            CF-Cache-Status: DYNAMIC
            Set-Cookie: __cf_bm=Qc7Bmv2jFNAACsyHMNVilFyRcdP0otO5Vh6fIenIF58-1718652590-1.0.1.1-opt4QPCpcTC988a5M76D_bjPDSgxOFsqPyTjaDqnDqYilp2KwcfCrXaHbveJRD6yD2SoH4D_SDi3opanlUFjqQ; path=/; expires=Mon, 17-Jun-24 19:59:50 GMT; domain=.api.openai.com; HttpOnly; Secure; SameSite=None, _cfuvid=CaQj2xRIYdEe8aYXxPZFNTdgDyiUy2NFNejDyk3VS_s-1718652590385-0.0.1.1-604800000; path=/; domain=.api.openai.com; HttpOnly; Secure; SameSite=None
            Server: cloudflare
            CF-RAY: 89557356e808766a-SEA
            Alt-Svc: h3=":443"
            Content-Type: application/json

      info: Polly[3]
            Execution attempt. Source: '-standard//Standard-Retry', Operation Key: '', Result: '200', Handled: 'False', Attempt: '0', Execution Time: '2176.1582'
      dbug: Polly[2]
            Resilience pipeline executed. Source: '-standard/', Operation Key: '', Result: '200', Execution Time: 2191.4154ms
      info: System.Net.Http.HttpClient.Default.LogicalHandler[101]
            End processing HTTP request after 2224.9318ms - 200
      trce: System.Net.Http.HttpClient.Default.LogicalHandler[103]
            Response Headers:
            Date: Mon, 17 Jun 2024 19:29:50 GMT
            Transfer-Encoding: chunked
            Connection: keep-alive
            openai-organization: jakerad
            openai-processing-ms: 1644
            openai-version: 2020-10-01
            Strict-Transport-Security: max-age=15724800; includeSubDomains
            x-ratelimit-limit-requests: 500
            x-ratelimit-limit-tokens: 30000
            x-ratelimit-remaining-requests: 499
            x-ratelimit-remaining-tokens: 29968
            x-ratelimit-reset-requests: 120ms
            x-ratelimit-reset-tokens: 64ms
            X-Request-ID: req_ca74fa8e36b14b8b8abf2c5d7a771662
            CF-Cache-Status: DYNAMIC
            Set-Cookie: __cf_bm=Qc7Bmv2jFNAACsyHMNVilFyRcdP0otO5Vh6fIenIF58-1718652590-1.0.1.1-opt4QPCpcTC988a5M76D_bjPDSgxOFsqPyTjaDqnDqYilp2KwcfCrXaHbveJRD6yD2SoH4D_SDi3opanlUFjqQ; path=/; expires=Mon, 17-Jun-24 19:59:50 GMT; domain=.api.openai.com; HttpOnly; Secure; SameSite=None, _cfuvid=CaQj2xRIYdEe8aYXxPZFNTdgDyiUy2NFNejDyk3VS_s-1718652590385-0.0.1.1-604800000; path=/; domain=.api.openai.com; HttpOnly; Secure; SameSite=None
            Server: cloudflare
            CF-RAY: 89557356e808766a-SEA
            Alt-Svc: h3=":443"
            Content-Type: application/json

      info: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            Prompt tokens: 165. Completion tokens: 24. Total tokens: 189.
      dbug: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            Tool requests: 1
      trce: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            Function call requests: WebSearchEnginePlugin-Search({"query":"major Microsoft announcements Build 2024"})
      info: Search[0]
            Function Search invoking.
      trce: Search[0]
            Function arguments: {"query":"major Microsoft announcements Build 2024"}
      Allow Search?
      y
      info: Search[0]
            Function Search succeeded.
      trce: Search[0]
            Function result: ["Welcome to Microsoft Build, our annual flagship event for developers, and to this year's edition of the Book of News. Here, you'll discover about 60 announcements, ranging from the latest AI features for Windows to the expansion of Microsoft Copilot and its new capabilities alongside novel tools for developers and cost-efficient and user-friendly cloud solutions for innovation.","Microsoft Build 2024 announcements - Surface Pro 11, Surface Laptop 7, Copilot+ PCs, huge AI upgrades and latest news Here are all the huge updates coming to Copilot+ PCs News.","Explore the .NET sessions at Microsoft Build 2024 to see the new features in action, or try them yourself by downloading .NET 9 Preview 4 today. Here's a look at our updates & announcements: Artificial Intelligence: End-to-end scenarios for building AI-enabled applications, embracing the AI ecosystem, and deep integration with cloud services.","Microsoft Build 2024, a major event for the Redmond-based tech giant, announced a number of new features on Tuesday, including updates to its AI chatbot Copilot, new Microsoft Teams tools, and ...","Microsoft Build, our flagship developer conference, wrapped up last Thursday. From the opening keynote from Satya Nadella to over 55 product announcements, our team has gone through them all and curated the top announcements and on-demand sessions for startup founders. GPT-4o generally available: Use your Azure credits to build and deploy ...","He spent over 15 years in IT support before joining The Verge. Microsoft is kicking off its three-day Build developer conference on Tuesday, May 21st, with a livestream starting at 11:30AM ET / 8 ...","May 21, 2024, 11:58 AM PDT. Microsoft CEO Satya Nadella at Build 2024. Screenshot: YouTube. Microsoft had a lot to say about Windows and AI - and a little to say about custom emoji - during ...","Here's a glimpse at our key announcements for developers that are available today and coming soon. 1. Copilot extensions. There are three ways to extend Copilot capabilities through connectors, plugins, and your own copilots. These three avenues of extensibility enable you to ground, customize, and enhance Copilot experiences with data and ...","With Recall, Microsoft is using AI to fix Windows' eternally broken search. At its Build 2024 conference, Microsoft unveiled Rewind, a new feature that aims to make local Windows PC searches as ...","With that introduction to Build in mind, let's explore some of the news and announcements. In-the-moment decision making with Real-Time Intelligence. For the most efficient AI apps, businesses need to be able to qualify, analyze and organize data at ingestion. This has proven to be a difficult step."]
      info: Search[0]
            Function completed. Duration: 54.0860011s
      info: System.Net.Http.HttpClient.Default.LogicalHandler[100]
            Start processing HTTP request POST https://api.openai.com/v1/chat/completions
      trce: System.Net.Http.HttpClient.Default.LogicalHandler[102]
            Request Headers:
            Accept: application/json
            Semantic-Kernel-Version: 1.13.0.0
            x-ms-client-request-id: 0d6d9301-2a78-4ea6-aa3a-2161a050b551
            x-ms-return-client-request-id: true
            User-Agent: Semantic-Kernel, azsdk-net-AI.OpenAI/1.0.0-beta.15, (.NET 8.0.5; Microsoft Windows 10.0.22631)
            Authorization: *
            Content-Type: application/json

      dbug: Polly[1]
            Resilience pipeline executing. Source: '-standard/', Operation Key: '(null)'
      info: System.Net.Http.HttpClient.Default.ClientHandler[100]
            Sending HTTP request POST https://api.openai.com/v1/chat/completions
      trce: System.Net.Http.HttpClient.Default.ClientHandler[102]
            Request Headers:
            Accept: application/json
            Semantic-Kernel-Version: 1.13.0.0
            x-ms-client-request-id: 0d6d9301-2a78-4ea6-aa3a-2161a050b551
            x-ms-return-client-request-id: true
            User-Agent: Semantic-Kernel, azsdk-net-AI.OpenAI/1.0.0-beta.15, (.NET 8.0.5; Microsoft Windows 10.0.22631)
            Authorization: *
            Content-Type: application/json

      info: System.Net.Http.HttpClient.Default.ClientHandler[101]
            Received HTTP response headers after 9370.0139ms - 200
      trce: System.Net.Http.HttpClient.Default.ClientHandler[103]
            Response Headers:
            Date: Mon, 17 Jun 2024 19:30:53 GMT
            Transfer-Encoding: chunked
            Connection: keep-alive
            openai-organization: jakerad
            openai-processing-ms: 9081
            openai-version: 2020-10-01
            Strict-Transport-Security: max-age=15724800; includeSubDomains
            x-ratelimit-limit-requests: 500
            x-ratelimit-limit-tokens: 30000
            x-ratelimit-remaining-requests: 499
            x-ratelimit-remaining-tokens: 29286
            x-ratelimit-reset-requests: 120ms
            x-ratelimit-reset-tokens: 1.428s
            X-Request-ID: req_d2364bb08052043963f866fce4212356
            CF-Cache-Status: DYNAMIC
            Server: cloudflare
            CF-RAY: 895574b51ce0766a-SEA
            Alt-Svc: h3=":443"
            Content-Type: application/json

      info: Polly[3]
            Execution attempt. Source: '-standard//Standard-Retry', Operation Key: '', Result: '200', Handled: 'False', Attempt: '0', Execution Time: '9370.5365'
      dbug: Polly[2]
            Resilience pipeline executed. Source: '-standard/', Operation Key: '', Result: '200', Execution Time: 9370.7481ms
      info: System.Net.Http.HttpClient.Default.LogicalHandler[101]
            End processing HTTP request after 9370.9569ms - 200
      trce: System.Net.Http.HttpClient.Default.LogicalHandler[103]
            Response Headers:
            Date: Mon, 17 Jun 2024 19:30:53 GMT
            Transfer-Encoding: chunked
            Connection: keep-alive
            openai-organization: jakerad
            openai-processing-ms: 9081
            openai-version: 2020-10-01
            Strict-Transport-Security: max-age=15724800; includeSubDomains
            x-ratelimit-limit-requests: 500
            x-ratelimit-limit-tokens: 30000
            x-ratelimit-remaining-requests: 499
            x-ratelimit-remaining-tokens: 29286
            x-ratelimit-reset-requests: 120ms
            x-ratelimit-reset-tokens: 1.428s
            X-Request-ID: req_d2364bb08052043963f866fce4212356
            CF-Cache-Status: DYNAMIC
            Server: cloudflare
            CF-RAY: 895574b51ce0766a-SEA
            Alt-Svc: h3=":443"
            Content-Type: application/json

      info: Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIChatCompletionService[0]
            Prompt tokens: 772. Completion tokens: 249. Total tokens: 1021.
      Here are some major announcements from the Microsoft Build 2024 event:

      1. **AI Features for Windows**: Microsoft introduced a range of new AI capabilities, including "Rewind" which improves local search functions on Windows PCs, and "Recall" aimed at fixing persistent issues with Windows search.

      2. **Microsoft Copilot Enhancements**: The expansion of Microsoft Copilot capabilities was announced, introducing new tools and enhancements to increase productivity and integration across various platforms.

      3. **Surface Hardware Updates**: Announcements included the launch of Surface Pro 11 and Surface Laptop 7, along with significant updates to the Copilot+ PCs.

      4. **Cloud Solutions**: Microsoft unveiled more cost-efficient and user-friendly cloud solutions to facilitate innovation.

      5. **Developer Tools**: New developer tools were showcased, including .NET 9 Preview 4, and ways to extend Copilot capabilities through connectors, plugins, and custom copilots.

      6. **GPT-4o Availability**: The event marked the general availability of GPT-4o, allowing developers to use Azure credits to build and deploy applications.

      These announcements reflect Microsoft's focus on integrating AI more deeply into its products and services, enhancing device offerings, and providing robust tools for developers.
      Q: dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[103]
            HttpMessageHandler expired after 120000ms for client ''
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.0639ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.0038ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.012ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.0132ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.0051ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.004ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.0045ms - processed: 0 items - remaining: 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[100]
            Starting HttpMessageHandler cleanup cycle with 1 items
      dbug: Microsoft.Extensions.Http.DefaultHttpClientFactory[101]
            Ending HttpMessageHandler cleanup cycle after 0.0036ms - processed: 0 items - remaining: 1 items
      ```

## Complete sample project

View the completed sample in the [06 Modifying Kernel Behavior with Dependency Injection](../../06%20-%20Modifying%20Kernel%20Behavior%20with%20Dependency%20Injection/)) project.

## Next unit: Exercise - Using Semantic kernel with Web App

[Continue](./07%20Using%20Semantic%20Kernel%20in%20WebApp.md)