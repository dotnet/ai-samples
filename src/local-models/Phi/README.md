## Torchsharp Phi

This repo contains a torchsharp implementation for phi model.

## Quick Start (Use Phi-2 model as an example)
To run the Phi model on your local machine, the following prerequisites are required:
- dotnet 6 or above
- git lfs, this is to download the model file from hugging face

### Step 1: Get the model weight from huggingface
To get Phi-2 model weight, run the following command to download model weight from huggingface. Be sure to have git lfs installed.
```bash
git clone https://huggingface.co/microsoft/phi-2
```
> [!Note]
> To run Phi-2 model on GPU, it's recommended to use a machine with at least 8GB of GPU memory.

> [!Note]
> Loading other Phi model should be similar but I haven't test them yet. Please create an issue if you have trouble loading other Phi models.

### Step 2: Run the model
Clone this repo and replace the `phi2Folder` folder with where you download huggingface model weight in [Program.cs](./Program.cs#L13)

Then run the following command to start the model:
```bash
dotnet run
```

## Intended Uses

### Chat Format
```csharp
var chatPrompt = @"Alice: I don't know why, I'm struggling to maintain focus while studying. Any suggestions?
Bob: Well, have you tried creating a study schedule and sticking to it?
Alice: Yes, I have, but it doesn't seem to help much.
Bob: Hmm, maybe you should try studying in a quiet environment, like the library.
Alice:";
var chatOutput = phi2.Generate(tokenizer, chatPrompt, maxLen: 256, temperature: 0.3f, stopSequences: [ "Bob:"]);
Console.WriteLine(chatOutput);
```
#### Output (include original prompt)
```
Alice: I don't know why, I'm struggling to maintain focus while studying. Any suggestions?
Bob: Well, have you tried creating a study schedule and sticking to it?
Alice: Yes, I have, but it doesn't seem to help much.
Bob: Hmm, maybe you should try studying in a quiet environment, like the library.
Alice: That's a good idea. I'll give it a try. Thanks, Bob!
```

### QA Format
```csharp
var prompt = @"Instruction: A skier slides down a frictionless slope of height 40m and length 80m, what's the skier's speed at the bottom?
Output:";
var output = phi2.Generate(tokenizer, prompt, maxLen: 256, temperature: 0.1f);
Console.WriteLine(output);
```

#### Output (include original prompt)
```
Instruction: A skier slides down a frictionless slope of height 40m and length 80m, what's the skier's speed at the bottom?!
Output: The skier's speed at the bottom of the slope can be calculated using the conservation of mechanical energy. At the top of the slope, the skier has gravitational potential energy (PE) and no kinetic energy (KE). At the bottom of the slope, the skier has only kinetic energy (KE) and no potential energy (PE). Therefore, the initial potential energy is equal to the final kinetic energy:

PE(top) = KE(bottom)
mgh = (1/2)mv^2

where m is the mass of the skier, g is the acceleration due to gravity, h is the height of the slope, and v is the speed of the skier at the bottom.

Solving for v, we get:

v = sqrt(2gh)

Plugging in the given values, we get:

v = sqrt(2 * 9.8 * 40)
v = 28.28 m/s

So, the skier's speed at the bottom of the slope is 28.28 m/s
```

### Code Format
```csharp
var codePrompt = @"Complete the following code
```python
def print_prime(n):
    # print all prime numbers less than n";
var codeOutput = phi2.Generate(tokenizer, codePrompt, maxLen: 256, temperature: 0f, stopSequences: [ "```"]);
Console.WriteLine(codeOutput);
```

#### Output (include original prompt)
```
Complete the following code
```python
def print_prime(n):
    # print all prime numbers less than n
    for i in range(2, n):
        for j in range(2, i):
            if i % j == 0:
                break
        else:
            print(i)

print_prime(10)
```

## Further reading: What's Phi?
Phi model is a suite of small language models developed by Microsoft Research. Until January 2024, there are three models available as follows:
- [Phi-2](https://huggingface.co/microsoft/phi-2), A 2.7B parameter model trained using the same data source with Phi-1.5 and augmented with a new data source that consists of various NLP synthetic texts and filtered websites (for safety and educational value).
- [Phi-1.5](https://huggingface.co/microsoft/phi-1_5), A 1.3B parameter model which demonstrates a nearly state-of-the-art performance among models with less than 10 billion parameters.
- [Phi-1](https://huggingface.co/microsoft/phi-1), A 1.3B parameter model, specialized for basic Python coding.

> [!Warning]
> All the Phi-series models does not fined tuned with RLHF, which means the model might generate offensive content. Please use with caution.
