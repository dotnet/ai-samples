This project demonstrates how to use the Microsoft Semantic Kernel to build a chat completion service integrated with OpenAI's GPT models and Bing Web Search. The code showcases advanced features such as adding services using Dependency Injection (DI), configuring HTTP client defaults, and implementing a permission filter.
### Configuring HTTP Client Defaults
The HTTP client configuration is extended to include a standard resilience handler and redact sensitive headers, such as Authorization, from being logged. Additionally, a redaction service is added.
```csharp
builder.Services.ConfigureHttpClientDefaults(b =>
{
    b.AddStandardResilienceHandler();
    b.RedactLoggedHeaders(["Authorization"]);
});
builder.Services.AddRedaction();
```
### Implementing a Permission Filter
A permission filter is added to the kernel services to control the invocation of kernel functions. The filter prompts the user for permission before executing a function.
```chsarp
# pragma warning disable
builder.Services.AddSingleton<IFunctionInvocationFilter, PermissionFilter>();
#pragma warning restore
```
The `PermissionFilter` class implements the `IFunctionInvocationFilter` interface. It asks the user for permission to execute a function and either proceeds or throws an exception based on the user's input.

###
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
````
