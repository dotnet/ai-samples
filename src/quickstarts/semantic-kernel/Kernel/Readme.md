
<p> This project demonstrates a simple console application using the Microsoft Semantic Kernel to interact with the OpenAI GPT-3.5-turbo model. The application allows users to have a stateless chat with the AI, meaning the AI does not retain memory of previous messages in the conversation.</>

## Code Overview
The main code utilizes the Microsoft Semantic Kernel to integrate with the OpenAI GPT-3.5-turbo model. Here's a breakdown of what the code does:

<ol> 
<li> Import the Microsoft Semantic Kernel:

```csharp
using Microsoft.SemanticKernel;
```
<li> Define the OpenAI Chat Completion Model Name:

```csharp
string openAIChatCompletionModelName = "gpt-3.5-turbo";
```
<li>Create and configure the kernel:

```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();
```
<ul> 
<li>`Kernel.CreateBuilder()`: Initializes the kernel builder.
<li> `AddOpenAIChatCompletion(...)`: Adds the OpenAI chat completion service using the specified model and API key.
<li> Build(): Builds the kernel with the configured services.
</ul> 

<li> Basic Chat Loop:

```Csharp
while (true)
{
    Console.Write("Q: ");
    Console.WriteLine(await kernel.InvokePromptAsync(Console.ReadLine()!));
}
```
<ul> 
<li> This loop continuously prompts the user for input, sends the input to the OpenAI model, and prints the AI's response.
</ul>
 </ol>


 ## Notes
 <ul>
 <li> This application is stateless, meaning the AI does not remember any previous interactions. Each input is treated independently.
 <li>Ensure that your environment variable <b>`OPENAI_API_KEY`</b> is correctly set up to avoid authentication issues.

 </ul>