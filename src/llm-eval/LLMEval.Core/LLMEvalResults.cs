namespace LLMEval.Core;

public class LLMEvalResults
{
    public string? EvalRunName { get; set; }

    public IList<LLMEvalPromptOutput> EvalResults { get; set; } = new List<LLMEvalPromptOutput>();
}