// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TorchSharp;
using static TorchSharp.torch;

public static class Extension
{
    public static string Generate(
        this PhiForCasualLM phi,
        BPETokenizer tokenizer,
        string prompt,
        int maxLen = 128,
        float temperature = 0.7f,
        float topP = 0.9f,
        string[]? stopSequences = null)
    {
        var inputIds = tokenizer.Encode(prompt);
        var inputTensor = torch.tensor(inputIds.ToArray(), dtype: ScalarType.Int64, device: phi.Device).unsqueeze(0);
        var attentionMask = torch.ones_like(inputTensor);
        var stopTokenIds = stopSequences == null ? [[tokenizer.EosId]] : stopSequences.Select(x => tokenizer.Encode(x)).ToArray();
        (var token, var _) = phi.Generate(inputTensor, attentionMask, temperature: temperature, maxLen: maxLen, topP: topP, stopTokenSequence: stopTokenIds);

        var tokenIds = token[0].to_type(ScalarType.Int32).data<int>().ToArray();
        var output = tokenizer.Decode(tokenIds);
        return output;

    }

    public static void Peek(this Tensor tensor, string id, int n = 10)
    {
        var device = tensor.device;
        tensor = tensor.cpu();
        var shapeString = string.Join(',', tensor.shape);
        var dataString = string.Join(',', tensor.reshape(-1)[..n].to_type(ScalarType.Float32).data<float>().ToArray());
        var tensor_1d = tensor.reshape(-1);
        var tensor_index = torch.arange(tensor_1d.shape[0], dtype: ScalarType.Float32).to(tensor_1d.device).sqrt();
        var avg = (tensor_1d * tensor_index).sum();
        avg = avg / tensor_1d.sum();
        Console.WriteLine($"{id}: sum: {avg.ToSingle()}  dtype: {tensor.dtype} shape: [{shapeString}] device: {device} has grad? {tensor.requires_grad}");
    }

    public static void Peek(this nn.Module model)
    {
        var state_dict = model.state_dict();
        // preview state_dict
        foreach (var (key, value) in state_dict.OrderBy(x => x.Key))
        {
            value.Peek(key);
        }
    }
}
