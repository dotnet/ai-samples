using Microsoft.SemanticKernel;

namespace LLMEval.Core;

public class RelevanceEval : PromptScoreEval
{
    public RelevanceEval(Kernel kernel) : base("relevance", kernel, "_prompts.relevance.skprompt.txt")
    {
    }
}
