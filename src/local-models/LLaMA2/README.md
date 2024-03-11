## Torchsharp LLaMA

Inspired by [pytorch-llama](https://github.com/hkproj/pytorch-llama), this project implements LLaMA 2 from scratch with [TorchSharp](https://github.com/dotnet/TorchSharp)

## Prerequisites

- .NET 6.0 SDK
- Access to one of LLaMA 2 models

## How to run

- Follow this [instruction](https://github.com/dotnet/TorchSharp/wiki/Sharing-Model-Data-between-PyTorch-and-TorchSharp) to convert llama2 model to torchsharp format.

> [!NOTE]
> torchsharp format seems not to support `torch.half` type yet, so the model weight is saved as `torch.float` type instead, which makes the model size twice as large as the original one. This issue is addressed in [this issue](https://github.com/dotnet/TorchSharp/issues/1204), and when loading model from torchsharp model file, the model weight will need to be converted to `torch.half` type manually.

- Change the path in `Program.cs` to your model file path.
- Run the project

## About tokenizer
This project uses a BPE tokenizer from [`Microsoft.ML.Tokenizer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.tokenizers.tokenizer?view=ml-dotnet-preview) to tokenize the input text. You can find the `vocab.json` and `merges.txt` under [torcharp-llama](Torchsharp-llama). To use a third-party tokenizer, you can simply replace the `vocab.json` and `merges.txt` with your own tokenizer files.

## Disclaimer
This project is only tested with LLaMA-2-7B model. I do hope I can have the chance to test it with other models, but unfortunately 7B model is already the largest model I can afford to run on my machine. If you have chance to test other models, please let me know if it works or not. Thanks!

Also, this project doesn't come with any warranty. Use it at your own risk.

## TODO
- [ ] Add support to load from `.safetensor` and native ckpt file so that we don't need to convert the model to torchsharp format. The support for `.safetensor` should be an easy one, but the support for native ckpt file is a bit tricky (otherwise why torchsharp format exists in the first place)
- [ ] Add support for lora training
