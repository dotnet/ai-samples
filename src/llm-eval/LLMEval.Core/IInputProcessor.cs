namespace LLMEval.Core;

public interface IInputProcessor
{
    public Task<ModelOutput> Process<T>(T item);
    public Task<List<ModelOutput>> ProcessCollection<T>(T collection);
}