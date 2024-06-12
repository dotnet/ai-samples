This project demonstrates a simple console application using the Microsoft Semantic Kernel to interact with the OpenAI GPT-3.5-turbo model. The application allows users to have a stateless chat with the AI, meaning the AI does not retain memory of previous messages in the conversation.

## Overview
The main code utilizes the Microsoft Semantic Kernel library to integrate with the OpenAI GPT-3.5-turbo model. Here's a breakdown of what the code does:
1.Import the core namespace for the Semantic Kernel library:
```csharp
using Microsoft.SemanticKernel;
```
2. Define the OpenAI Chat Completion Model Name:
```csharp
string openAIChatCompletionModelName = "gpt-3.5-turbo";
```
3.Create and configure the kernel:

```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();
```
- `Kernel.CreateBuilder()`: Initializes the kernel builder.
-  `AddOpenAIChatCompletion(...)`: Adds the OpenAI chat completion service using the specified model and API key.
-  Build(): Builds the kernel with the configured services.

4.  Basic Chat Loop:

```Csharp
while (true)
{
    Console.Write("Q: ");
    Console.WriteLine(await kernel.InvokePromptAsync(Console.ReadLine()!));
}
```
-  This loop continuously prompts the user for input, sends the input to the OpenAI model, and prints the AI's response.

 ## Notes
 -  This application is stateless, meaning the AI does not remember any previous interactions. Each input is treated independently.


 ### Next unit: Exercise - Add Chat History
[Continue](../02%20-%20Add%20Chat%20History/README.md)
