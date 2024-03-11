using TorchSharp;
using static TorchSharp.torch;

public static class Extension
{
    public static void Peek(this Tensor tensor, string id, int n = 10)
    {
        var shapeString = string.Join(',', tensor.shape);
        var dataString = string.Join(',', tensor.reshape(-1)[0..n].to_type(ScalarType.Float32).data<float>().ToArray());
        Console.WriteLine($"{id}: tensor [{dataString}] dtype: {tensor.dtype} shape: {shapeString} device: {tensor.device}");
    }

    public static void LoadStateDict(this Dictionary<string, Tensor> dict, string location)
    {
        using FileStream stream = File.OpenRead(location);
        using BinaryReader reader = new BinaryReader(stream);
        var num = reader.Decode();
        Console.WriteLine($"num: {num}");
        for (int i = 0; i < num; i++)
        {
            var key = reader.ReadString();
            Tensor tensor = dict[key];
            Console.WriteLine($"load key: {key} tensor: {tensor}");

            var originalDevice = tensor.device;
            var originalType = tensor.dtype;
            if (tensor.dtype == ScalarType.BFloat16)
            {
                tensor = tensor.to_type(ScalarType.Float32);
            }

            TensorExtensionMethods.Load(ref tensor, reader, skip: false);
            
            // convert type to bf16 if type is float
            tensor = tensor.to_type(originalType);
            dict[key] = tensor;
        }
    }

    //
    // 摘要:
    //     Decode a long value from a binary reader
    //
    // 参数:
    //   reader:
    //     A BinaryReader instance used for input.
    //
    // 返回结果:
    //     The decoded value
    public static long Decode(this BinaryReader reader)
    {
        long num = 0L;
        int num2 = 0;
        while (true)
        {
            long num3 = reader.ReadByte();
            num += (num3 & 0x7F) << num2 * 7;
            if ((num3 & 0x80) == 0L)
            {
                break;
            }

            num2++;
        }

        return num;
    }
}