using LLMEval.Data;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace QAGenerator;

public class QASkill
{
    private readonly KernelFunction _createQAFunction;

    private readonly Kernel _kernel;

    public static QASkill Create(Kernel kernel)
    {
        string promptTemplate = EmbeddedResource.Read("_prompts.qa.skprompt.txt")!;

        return new QASkill(kernel, kernel.CreateFunctionFromPrompt(promptTemplate));
    }

    public QASkill(Kernel kernel, KernelFunction promptFunction)
    {
        _createQAFunction = promptFunction;
        _kernel = kernel;
    }

    public async Task<QA?> GetQA(string question, string answer, string topic)
    {
        var context = new KernelArguments
        {
            { "question", question },
            { "answer", answer }
        };

        var result = await _createQAFunction.InvokeAsync(_kernel, context);
        var resultString = result.ToString();

        var qa = new QA();
        try
        {
            qa = JsonSerializer.Deserialize<QA>(resultString, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (JsonException jsonExc)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            qa.Question = "An error occurred while generating the QA. Error descripton";
            qa.Answer = jsonExc.Message.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        return qa;
    }
}