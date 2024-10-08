### Microsoft.Extensions.AI (preview)

Microsoft.Extensions.AI is a set of core .NET libraries developed in collaboration with the .NET ecosystem, including Semantic Kernel. These libraries provide a unified layer of C# abstractions for interacting with AI services, such as small and large language models (SLMs and LLMs) and embeddings.

Core benefits:

- *Unified API:* Offers a consistent and standard set of APIs and conventions for integrating AI services into .NET applications.
- *Flexibility:* Allows .NET library authors to use AI services without forcing a specific AI provider, making it parameterizable with any provider.
- *Ease of Use:* Enables .NET application developers to experiment with different packages using the same underlying abstractions and to utilize a single API throughout their application.
- *Componentization:* Facilitates the addition of new capabilities and simplifies the componentization and testing of applications.

For more details, see the [Introducing Microsoft.Extensions.AI Preview blog post](https://aka.ms/meai-preview-blog).

|Topic | GitHub Link | Description | 
| --- | --- | --- |
 Abstraction implementations | [GitHub Link](./abstraction-implementations/README.md) | Samples containing reference implementations of Microsoft.Extensions.AI.Abstractions |
| OpenAI | [GitHub Link](./openai/README.md) | Samples showcasing Microsoft.Extensions.AI.OpenAI reference implementation |
| Azure AI Inference | [GitHub Link](./azure-ai-inference/README.md) | Samples showcasing Microsoft.Extensions.AI.AzureAIInference reference implementation |
| Ollama | [GitHub Link](./ollama/README.md) | Samples showcasing Microsoft.Extensions.AI.Ollama reference implementation | 