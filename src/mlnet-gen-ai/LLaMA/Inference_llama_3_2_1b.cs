// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.AI;
using Microsoft.ML.GenAI.Core;
using Microsoft.ML.GenAI.LLaMA;
using Microsoft.ML.Tokenizers;
using TorchSharp;

public partial class LLaMASamples
{
    public static async Task Inference_llama_3_2_1B()
    {
        var device = "cuda";
        torch.InitializeDevice(device);

        // download model from huggingface model hub url and replace the modelFolder with your own model folder
        // huggingface model hub: https://huggingface.co/meta-llama/Llama-3.2-1B-Instruct/tree/main/
        var modelFolder = "/path/to/download/folder";
        var tokenizerModelFolder = Path.Combine(modelFolder, "original");

        var tokenizer = LlamaTokenizerHelper.FromPretrained(tokenizerModelFolder);
        var model = LlamaForCausalLM.FromPretrained(modelFolder, checkPointName: "model.safetensors", quantizeToInt8: true);

        // create CausalLMPipeline for inference
        var pipeline = new CausalLMPipeline<TiktokenTokenizer, LlamaForCausalLM>(tokenizer, model, device);

        var task = """
            Write a C# program to calculate the sum of all numbers from 1 to 100.
            """;

        // Inference using M.E.A.I IChatClient
        IChatClient client = new Llama3CausalLMChatClient(pipeline);

        var chatMessage = new ChatMessage(ChatRole.User, task);

        await foreach (var response in client.CompleteStreamingAsync([chatMessage]))
        {
            Console.Write(response.Text);
        }
    }
}
