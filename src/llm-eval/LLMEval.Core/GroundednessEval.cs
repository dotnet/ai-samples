using Microsoft.SemanticKernel;

namespace LLMEval.Core;

public class GroundednessEval : PromptScoreEval
{
    public GroundednessEval(Kernel kernel) : base("groundedness", kernel, "_prompts.groundedness.skprompt.txt")
    {
    }
}