using Microsoft.SemanticKernel;

var openAIChatCompletionModelName = "gpt-3.5-turbo"; // this could be other models like "gpt-4o".
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(openAIChatCompletionModelName, Environment.GetEnvironmentVariable("OPENAI_API_KEY")) // add the OpenAI chat completion service.
    .Build();

// Basic chat
// This is zero memory or stateless chat. The AI will not remember anything from the previous messages.
while (true)
{
    Console.Write("Q: ");
    Console.WriteLine(await kernel.InvokePromptAsync(Console.ReadLine()!));
}
