#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.


using LLMEval.Data;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace QAGenerator;

public class QALLMGenerator
{
    public static async Task<List<QA>> GenerateQACollection(Kernel kernel, int collectionCount = 5, string topic = "")
    {
        List<QA> res = new List<QA>();
        for (int i = 0; i < collectionCount; i++)
        {
            var qa = await GenerateQA(kernel, topic);
            res.Add(qa);
        }
        return res;
    }


    public static async Task<QA> GenerateQA(Kernel kernel, string topic = "")
    {
        var localFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var pluginsDirectoryPath = Path.Combine(localFolder, "_prompts");
        var plugins = kernel.CreatePluginFromPromptDirectory(pluginsDirectoryPath);

        var promptArgs = new KernelArguments
        {
            { "topic", topic }
        };

        var result = await kernel.InvokeAsync(plugins["qagen"], promptArgs);
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
            qa.Question = "An error occurred while generating the QA. Error descripton";
            qa.Answer = jsonExc.Message.ToString();
        }
        return qa;
    }
}
