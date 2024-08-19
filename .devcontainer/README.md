# DevContainer Configurations

## Base

These set of images contain:

- .NET 8 SDK
- .NET VS Code Extensions

Depending on the hardware you have, you can choose between:

- [CPU](./cpu/devcontainer.json)
- [GPU (NVIDIA)](./gpu/devcontainer.json)

## Ollama

In addition to the same configuration as the base dev container configurations, they also include Ollama for working with local models.

Depending on the hardware you have, you can choose between:

- [CPU](./ollama-cpu/devcontainer.json)
- [GPU (NVIDIA)](./ollama-gpu/devcontainer.json)
