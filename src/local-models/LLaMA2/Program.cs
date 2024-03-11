// See https://aka.ms/new-console-template for more information

using FluentAssertions;
using LLAMA;
using TorchSharp;
var vocabPath = @"vocab.json";
var mergesPath = @"merges.txt";
var tokenizer = new BPETokenizer(vocabPath, mergesPath);
var checkpointDirectory = @"path/to/llama-2-7b";
var device = "cuda";

if (device == "cuda")
{
    torch.InitializeDeviceType(DeviceType.CUDA);
    torch.cuda.is_available().Should().BeTrue();
}

torch.manual_seed(100);
var model = LLaMA.Build(
       checkPointsDirectory: checkpointDirectory,
       tokenizer: tokenizer,
       maxSeqLen: 128,
       maxBatchSize: 1,
       device: device);

var prompts = new[]
{
    "I believe the meaning of life is",
};
var result = model.TextCompletion(prompts, temperature: 0, echo: true, device: device);

foreach (var item in result)
{
    Console.WriteLine($"generation: {item.generation}");
}
