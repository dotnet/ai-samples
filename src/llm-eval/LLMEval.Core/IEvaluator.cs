namespace LLMEval.Core;

public interface IEvaluator<T>
{
    public string Id { get; }

    public Task<T> Eval(ModelOutput modelOutput);
}