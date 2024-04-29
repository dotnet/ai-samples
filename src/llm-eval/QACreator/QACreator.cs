#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

using LLMEval.Core;
using LLMEval.Data;
using Microsoft.SemanticKernel;
using QAGenerator;

namespace QACreator;

public class QACreator : IInputProcessor
{
    private readonly QASkill qaGenerator;

    public QACreator(Kernel kernel)
    {
        qaGenerator = QASkill.Create(kernel);
    }

    public async Task<List<ModelOutput>> ProcessCollection<T>(T collection)
    {
        var result = new List<ModelOutput>();
        foreach (var qa in collection as List<QA>)
        {
            var modelOutput = await Process(qa);
            result.Add(modelOutput);
        }
        return result;
    }

    public async Task<ModelOutput> Process<T>(T source)
    {
        var qa = source as QA;

        var modelResponse = await qaGenerator.GetQA(qa.Question, qa.Answer, qa.Topic);
        return new ModelOutput()
        {
            Input = $@"The model was asked this question: ""{qa.Question}"", expecting this answer: ""{qa.Answer}"", for this topic: ""{qa.Topic}""",
            Output = $@"The model answer is: ""{modelResponse.Answer}"""
        };
    }
}