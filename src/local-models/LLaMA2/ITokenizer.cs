using Microsoft.ML.Tokenizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLAMA;

public interface ITokenizer
{
    public int[] Encode(string input, bool bos, bool eos);

    public string Decode(int[] input);

    public int VocabSize { get; }

    public int PadId { get; }

    public int BosId { get; }

    public int EosId { get; }
}

public class Norm : Normalizer
{
    public override NormalizedString Normalize(string original)
    {
        // replace space with _
        var normalized = original.Replace(" ", "▁");

        return new NormalizedString(original, normalized, null, isOneToOneMapping: true);
    }
}

public class PreTokenizer : Microsoft.ML.Tokenizers.PreTokenizer
{
    public override IReadOnlyList<Split> PreTokenize(string sentence)
    {
        var split = new Split(sentence, new(0, sentence.Length));

        return new List<Split> { split };
    }
}

public class TokenizeDecoder : Microsoft.ML.Tokenizers.TokenizerDecoder
{
    private const char spaceReplacement = '▁';
    private string bos = "<s>";
    private string eos = "</s>";

    public TokenizeDecoder(string bos = "<s>", string eos = "</s>")
    {
        this.bos = bos;
        this.eos = eos;
    }

    public override string Decode(IEnumerable<string> tokens)
    {
        var str = string.Join("", tokens);
        str = str.Replace(spaceReplacement, ' ');

        if (str.StartsWith(bos))
        {
            str = str.Substring(bos.Length);
        }

        if (str.EndsWith(eos))
        {
            str = str.Substring(0, str.Length - eos.Length);
        }

        return str;
    }
}

public class BPETokenizer : ITokenizer
{
    private Tokenizer tokenizer;
    private bool addPrecedingSpace;

    public BPETokenizer(string vocabPath, string mergesPath, bool addPrecedingSpace = true, int padToken = -1, int startToken = 1, int endToken = 2)
    {
        this.BosId = startToken;
        this.EosId = endToken;
        this.addPrecedingSpace = addPrecedingSpace;
        this.PadId = padToken;
        var bpe = new Bpe(vocabPath, mergesPath);
        this.tokenizer = new Tokenizer(bpe, preTokenizer: new PreTokenizer(), normalizer: new Norm());
        var decoder = new TokenizeDecoder(this.tokenizer.Model.IdToToken(this.BosId)!, this.tokenizer.Model.IdToToken(this.EosId)!);
        this.tokenizer.Decoder = decoder;
    }
    public int VocabSize => this.tokenizer.Model.GetVocabSize();

    public int PadId { get; }

    public int BosId { get; }

    public int EosId { get; }

    public string Decode(int[] input)
    {
        var str = this.tokenizer.Decode(input) ?? throw new Exception("Failed to decode");
        if (this.addPrecedingSpace)
        {
            str = str.TrimStart();
        }

        return str;
    }

    public int[] Encode(string input, bool bos, bool eos)
    {
        if (this.addPrecedingSpace)
        {
            input = " " + input;
        }
        var tokens = this.tokenizer.Encode(input).Ids.ToArray();
        if (bos)
        {
            tokens = new int[] { this.BosId }.Concat(tokens).ToArray();
        }
        if (eos)
        {
            tokens = tokens.Concat(new int[] { this.EosId }).ToArray();
        }

        Console.WriteLine($"tokens: {string.Join(",", tokens)}");

        return tokens;
    }
}
