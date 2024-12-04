using Microsoft.SemanticKernel;

namespace LLMEval.Core;

public class CoherenceEval : PromptScoreEval
{
    public CoherenceEval(Kernel kernel) : base("coherence", kernel, "_prompts.coherence.skprompt.txt")
    {
    }
}