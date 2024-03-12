// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.PortableExecutable;
using System.Text.Json;
using Microsoft.ML.Tokenizers;

public class Norm : Normalizer
{
    public override NormalizedString Normalize(string original)
    {
        // replace space with Ġ
        var normalized = original.Replace(" ", "Ġ");

        // replace \r\n with Ċ
        normalized = normalized.Replace("\r\n", "č");

        // replace newline with Ċ
        normalized = normalized.Replace("\n", "Ċ");

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
    private const char spaceReplacement = 'Ġ';

    private const char newlineReplacement = 'Ċ';

    private const char carriageReturnReplacement = 'č';
    private readonly string bos = "<s>";
    private readonly string eos = "</s>";

    public TokenizeDecoder(string bos = "<s>", string eos = "</s>")
    {
        this.bos = bos;
        this.eos = eos;
    }

    public override string Decode(IEnumerable<string> tokens)
    {
        var str = string.Join("", tokens);
        str = str.Replace(spaceReplacement, ' ');
        str = str.Replace(newlineReplacement, '\n');
        str = str.Replace(carriageReturnReplacement.ToString(), Environment.NewLine);

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

public class BPETokenizer
{
    private readonly Tokenizer tokenizer;
    private readonly bool addPrecedingSpace;

    public BPETokenizer(
        string vocabPath,
        string mergesPath,
        bool addPrecedingSpace,
        string uknToken,
        string bosToken,
        string eosToken)
    {
        this.addPrecedingSpace = addPrecedingSpace;
        var bpe = new Bpe(vocabPath, mergesPath);
        this.tokenizer = new Tokenizer(bpe, preTokenizer: new PreTokenizer(), normalizer: new Norm());
        this.BosId = this.tokenizer.Model.TokenToId(bosToken) ?? throw new Exception("Failed to get bos id");
        this.EosId = this.tokenizer.Model.TokenToId(eosToken) ?? throw new Exception("Failed to get eos id");
        var decoder = new TokenizeDecoder(this.tokenizer.Model.IdToToken(this.BosId)!, this.tokenizer.Model.IdToToken(this.EosId)!);
        this.tokenizer.Decoder = decoder;
    }

    public static BPETokenizer FromPretrained(
        string folder,
        string vocabFile = "vocab.json",
        string mergesFile = "merges.txt",
        string specialTokensFile = "special_tokens_map.json",
        bool addPrecedingSpace = false,
        string uknToken = "<|endoftext|>",
        string bosToken = "<|endoftext|>",
        string eosToken = "<|endoftext|>")
    {
        var vocabPath = Path.Combine(folder, vocabFile);
        var mergesPath = Path.Combine(folder, mergesFile);
        var specialTokenMapPath = Path.Combine(folder, specialTokensFile);

        var specialTokenMap = new Dictionary<string, string>();
        if (File.Exists(Path.Combine(folder, specialTokensFile)))
        {
            specialTokenMap = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(specialTokenMapPath)) ?? throw new Exception("Failed to load special token map");
        }

        if (specialTokenMap.ContainsKey("bos_token"))
        {
            bosToken = specialTokenMap["bos_token"];
        }

        if (specialTokenMap.ContainsKey("eos_token"))
        {
            eosToken = specialTokenMap["eos_token"];
        }

        if (specialTokenMap.ContainsKey("unk_token"))
        {
            uknToken = specialTokenMap["unk_token"];
        }

        return new BPETokenizer(vocabPath, mergesPath, addPrecedingSpace, uknToken, bosToken, eosToken);
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

    public int TokenToId(string token)
    {
        return this.tokenizer.Model.TokenToId(token) ?? throw new Exception("Failed to get token id");
    }

    public int[] Encode(string input, bool bos = false, bool eos = false)
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
        return tokens;
    }
}
