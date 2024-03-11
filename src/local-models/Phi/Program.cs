using System.Runtime.InteropServices;
using FluentAssertions;
using TorchSharp;
using static TorchSharp.torch;

// Dynamic loading libtorch because Cuda 12 only support GPU driver >= 520
// And I can't upgrade GPU driver because it's a cloud machine.

// Comment out the following two line if your machine support Cuda 12
// var libTorch = "path/to/libtorch.so";
// NativeLibrary.Load(libTorch);

var phi2Folder = "../phi-2";
var device = "cpu";

if (device == "cuda")
{
    torch.InitializeDeviceType(DeviceType.CUDA);
    torch.cuda.is_available().Should().BeTrue();
}
var defaultType = ScalarType.Float32;
torch.set_default_dtype(defaultType);
torch.manual_seed(1);

var tokenizer = BPETokenizer.FromPretrained(phi2Folder);
var phi2 = PhiForCasualLM.FromPretrained(phi2Folder, device: device, defaultDType: defaultType, weightsName: "phi-2.pt");


// QA Format
Console.WriteLine("QA Format");
var prompt = @"Instruction: A skier slides down a frictionless slope of height 40m and length 80m, what's the skier's speed at the bottom?
Output:";
var output = phi2.Generate(tokenizer, prompt, maxLen: 512, temperature: 0.3f);
Console.WriteLine(output);

// Chat Format
Console.WriteLine("Chat Format");
var chatPrompt = @"Alice: I don't know why, I'm struggling to maintain focus while studying. Any suggestions?
Bob: Well, have you tried creating a study schedule and sticking to it?
Alice: Yes, I have, but it doesn't seem to help much.
Bob: Hmm, maybe you should try studying in a quiet environment, like the library.
Alice:";
var chatOutput = phi2.Generate(tokenizer, chatPrompt, maxLen: 256, temperature: 0.3f, stopSequences: [ "Bob:"]);
Console.WriteLine(chatOutput);

// Code Format
Console.WriteLine("Code Format");
var codePrompt = @"Complete the following code
```python
def print_prime(n):
    # print all prime numbers less than n";
var codeOutput = phi2.Generate(tokenizer, codePrompt, maxLen: 1024, temperature: 0f, stopSequences: [ "```"]);
Console.WriteLine(codeOutput);

