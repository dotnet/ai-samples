using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TorchSharp;
using TorchSharp.Modules;
using static System.Formats.Asn1.AsnWriter;
using static Tensorboard.TensorShapeProto.Types;
using static TorchSharp.torch;
namespace LLAMA;

public class ModelArgs
{
    [JsonPropertyName("dim")]
    public int Dim { get; set; } = 4096;

    [JsonPropertyName("n_layers")]
    public int NLayers { get; set; } = 32;

    [JsonPropertyName("n_heads")]
    public int NHeads { get; set; } = 32;

    [JsonPropertyName("n_kv_heads")]
    public int? NKVHeads { get; set; } = null;

    [JsonPropertyName("vocab_size")]
    public int VocabSize { get; set; } = -1;

    [JsonPropertyName("multiple_of")]
    public int MultipleOf { get; set; } = 256;

    [JsonPropertyName("ffn_dim_multiplier")]
    public float? FFNDimMultiplier { get; set; } = null;

    [JsonPropertyName("norm_eps")]
    public float NormEps { get; set; } = 1e-5f;

    [JsonPropertyName("max_batch_size")]
    public int MaxBatchSize { get; set; } = 3;

    [JsonPropertyName("max_seq_len")]
    public int MaxSeqLen { get; set; } = 1024;

    [JsonPropertyName("device")]
    public string Device { get; set; } = "cpu";

    public ScalarType Dtype => ScalarType.BFloat16;
}

public class RMSNorm : torch.nn.Module<Tensor, Tensor>
{
    private int _dim;
    private float _eps;
    private Parameter weight;

    public RMSNorm(ModelArgs args)
        : base(nameof(RMSNorm))
    {
        this._dim = args.Dim;
        this._eps = args.NormEps;

        // the gamma scalar
        this.weight = torch.nn.Parameter(torch.ones(args.Dim, dtype: args.Dtype));
    }

    private Tensor Norm(Tensor x)
    {
        // (B, Seq_Len, Dim) * (B, Seq_Len, 1) = (B, Seq_Len, Dim)
        // rsqrt = 1 / sqrt
        var output = x * torch.rsqrt(x.pow(2).mean([-1L], keepdim: true) + this._eps);
        return output;
    }

    public override Tensor forward(Tensor input)
    {
        // needs higher precision for the norm so convert to float32
        // (B, Seq_Len, Dim)
        var normed = this.Norm(input.to_type(ScalarType.Float32)).type_as(input);
        // (B, Seq_Len, Dim) * (Dim) = (B, Seq_Len, Dim)
        var output = this.weight * normed;

        return output;
    }
}

public class SelfAttention : torch.nn.Module<Tensor, int, Tensor, Tensor?, Tensor>
{
    private int nKVHeads;
    private int nHeadsQ;
    private int nRep;
    private int headDim;
    private Linear wq;
    private Linear wk;
    private Linear wv;
    private Linear wo;
    private Tensor cache_k;
    private Tensor cache_v;

    public SelfAttention(ModelArgs args)
        : base(nameof(SelfAttention))
    {
        // # Indicates the number of heads for the Keys and Values
        this.nKVHeads = args.NKVHeads ?? args.NHeads;
        // Indicates the number of heads for the Queries
        this.nHeadsQ = args.NHeads;
        // Indicates how many times the Keys and Values should be repeated
        this.nRep = this.nHeadsQ / this.nKVHeads;
        //Indicates the dimension of each head, that is, the part of the embedding that each head will be responsible for
        this.headDim = args.Dim / args.NHeads;

        this.wq = torch.nn.Linear(args.Dim, args.NHeads * this.headDim, hasBias: false, dtype: args.Dtype);
        this.wk = torch.nn.Linear(args.Dim, this.nKVHeads * this.headDim, hasBias: false, dtype: args.Dtype);
        this.wv = torch.nn.Linear(args.Dim, this.nKVHeads * this.headDim, hasBias: false, dtype: args.Dtype);
        this.wo = torch.nn.Linear(args.NHeads * this.headDim, args.Dim, hasBias: false, dtype: args.Dtype);
        RegisterComponents();

        this.cache_k = torch.zeros(args.MaxBatchSize, args.MaxSeqLen, this.nKVHeads, this.headDim, dtype: args.Dtype);
        this.cache_v = torch.zeros(args.MaxBatchSize, args.MaxSeqLen, this.nKVHeads, this.headDim, dtype: args.Dtype);
    }

    public override Tensor forward(Tensor input, int startPos, Tensor freqsComplex, Tensor? mask = null)
    {
        int batchSize = (int)input.shape[0];
        int seqLen = (int)input.shape[1];
        var dim = input.shape[2];

        // (B, Seq_Len, Dim) -> (B, Seq_Len, N_Heads * Head_Dim)
        var xq = this.wq.forward(input);

        // (B, Seq_Len, Dim) -> (B, Seq_Len, H_KV * Head_Dim)
        var xk = this.wk.forward(input);

        // (B, Seq_Len, Dim) -> (B, Seq_Len, H_KV * Head_Dim)
        var xv = this.wv.forward(input);

        // (B, 1, H_Q * Head_Dim) -> (B, 1, H_Q, Head_Dim)
        xq = xq.view(batchSize, seqLen, this.nHeadsQ, this.headDim);

        // (B, Seq_Len, H_KV * Head_Dim) -> (B, Seq_Len, H_KV, Head_Dim)
        xk = xk.view(batchSize, seqLen, this.nKVHeads, this.headDim);

        // (B, Seq_Len, H_KV * Head_Dim) -> (B, Seq_Len, H_KV, Head_Dim)
        xv = xv.view(batchSize, seqLen, this.nKVHeads, this.headDim);
        // (B, Seq_Len, H_Q, Head_Dim) -> (B, Seq_Len, H_Q, Head_Dim)
        xq = Utils.ApplyRotaryEmbeddings(xq, freqsComplex);

        // (B, Seq_Len, H_KV, Head_Dim) -> (B, Seq_Len, H_KV, Head_Dim)
        xk = Utils.ApplyRotaryEmbeddings(xk, freqsComplex);

        // replace entries in cache
        this.cache_k[..batchSize, startPos..(startPos + seqLen)] = xk;
        this.cache_v[..batchSize, startPos..(startPos + seqLen)] = xv;

        var keys = this.cache_k[..batchSize, ..(startPos + seqLen)];
        var values = this.cache_v[..batchSize, ..(startPos + seqLen)];

        // Since every group of Q shares the same K and V heads, just repeat the K and V heads for every Q in the same group.
        // (B, Seq_Len, H_KV, Head_Dim) -> (B, Seq_Len_KV, H_Q, Head_Dim)
        keys = Utils.RepeatKV(keys, this.nRep);

        // (B, Seq_Len, H_KV, Head_Dim) -> (B, Seq_Len_KV, H_Q, Head_Dim)
        values = Utils.RepeatKV(values, this.nRep);

        // (B, Seq_Len, H_Q, Head_Dim) -> (B, H_Q, Seq_Len, Head_Dim)
        xq = xq.transpose(1, 2);

        // (B, Seq_Len_KV, H_Q, Head_Dim) -> (B, H_Q, Seq_Len_KV, Head_Dim)
        keys = keys.transpose(1, 2);

        // (B, Seq_Len_KV, H_Q, Head_Dim) -> (B, H_Q, Seq_Len_KV, Head_Dim)
        values = values.transpose(1, 2);
        // (B, H_Q, Seq_Len, Head_Dim) @ (B, H_Q, Head_Dim, Seq_Len_KV) -> (B, H_Q, Seq_Len, Seq_Len_KV)
        var scores = torch.matmul(xq, keys.transpose(2, 3)) / Math.Sqrt(this.headDim);
        if (mask is not null)
        {
            scores = scores + mask;
        }

        var softmax = torch.nn.functional.softmax(scores, dim: -1);

        // (B, H_Q, Seq_Len, Seq_Len_KV) @ (B, H_Q, Seq_Len_KV, Head_Dim) -> (B, H_Q, Seq_Len, Head_Dim)
        var output = torch.matmul(softmax, values);

        // (B, H_Q, Seq_Len, Head_Dim) -> (B, Seq_Len, H_Q, Head_Dim) -> (B, Seq_Len, Dim)
        output = output.transpose(1, 2).contiguous().view(batchSize, seqLen, -1);

        // (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        output = this.wo.forward(output);

        return output;
    }
}

public class FeedForward : torch.nn.Module<Tensor, Tensor>
{
    private Linear w1;
    private Linear w2;
    private Linear w3;

    public FeedForward(ModelArgs args)
        : base(nameof(FeedForward))
    {
        var hiddenDim = args.Dim * 4;
        hiddenDim = 2 * hiddenDim / 3;
        hiddenDim = args.FFNDimMultiplier.HasValue ? (int)args.FFNDimMultiplier.Value * hiddenDim : hiddenDim;

        // Round the hidden_dim to the nearest multiple of the multiple_of parameter
        hiddenDim = args.MultipleOf * ((hiddenDim + args.MultipleOf - 1) / args.MultipleOf);
        this.w1 = torch.nn.Linear(args.Dim, hiddenDim, hasBias: false, dtype: args.Dtype);
        this.w2 = torch.nn.Linear(hiddenDim, args.Dim, hasBias: false, dtype: args.Dtype);
        this.w3 = torch.nn.Linear(args.Dim, hiddenDim, hasBias: false, dtype: args.Dtype);

        RegisterComponents();
    }

    public override Tensor forward(Tensor input)
    {
        // (B, Seq_Len, Dim) -> (B, Seq_Len, Hidden_Dim)
        var swish = torch.nn.functional.silu(this.w1.forward(input));
        // (B, Seq_Len, Hidden_Dim) -> (B, Seq_Len, Dim)
        var xV = this.w3.forward(input);
        // (B, Seq_Len, Hidden_Dim) * (B, Seq_Len, Hidden_Dim) -> (B, Seq_Len, Hidden_Dim)
        var x = swish * xV;
        // (B, Seq_Len, Hidden_Dim) -> (B, Seq_Len, Dim)
        x = this.w2.forward(x);

        return x;
    }
}

public class EncoderBlock : torch.nn.Module<Tensor, int, Tensor, Tensor?, Tensor>
{
    private SelfAttention attention;
    private FeedForward feed_forward;
    private RMSNorm attention_norm;
    private RMSNorm ffn_norm;

    public EncoderBlock(ModelArgs args)
        : base(nameof(EncoderBlock))
    {
        this.attention = new SelfAttention(args);
        this.feed_forward = new FeedForward(args);
        this.attention_norm = new RMSNorm(args);
        this.ffn_norm = new RMSNorm(args);
    }

    public override Tensor forward(Tensor input, int startPos, Tensor freqsComplex, Tensor? mask)
    {
        // (B, Seq_Len, Dim) + (B, Seq_Len, Dim) --> (B, Seq_Len, Dim)
        var x = this.attention_norm.forward(input);
        // (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        x = this.attention.forward(x, startPos, freqsComplex, mask);
        // (B, Seq_Len, Dim) + (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        var h = x + input;
        // (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        x = this.ffn_norm.forward(h);
        // (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        x = this.feed_forward.forward(x);
        // (B, Seq_Len, Dim) + (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        x = x + h;

        return x;
    }
}

public class Transformer : nn.Module<Tensor, int, Tensor>
{
    private ModelArgs args;
    private int vocabSize;
    private int nLayers;
    private Embedding tok_embeddings;
    private ModuleList<nn.Module<Tensor, int, Tensor, Tensor?, Tensor>> layers;
    private RMSNorm norm;
    private Linear output;
    private Tensor freqs_compex;

    public Transformer(ModelArgs args)
        : base(nameof(Transformer))
    {
        args.VocabSize.Should().BeGreaterThan(0, "Vocab size must be set");

        this.args = args;
        this.vocabSize = args.VocabSize;
        this.nLayers = args.NLayers;
        this.tok_embeddings = nn.Embedding(this.vocabSize, this.args.Dim, dtype: args.Dtype);

        this.layers = nn.ModuleList<nn.Module<Tensor, int, Tensor, Tensor?, Tensor>>();
        for (int i = 0; i < this.nLayers; i++)
        {
            this.layers.Add(new EncoderBlock(args));
        }

        this.norm = new RMSNorm(args);
        this.output = nn.Linear(args.Dim, this.vocabSize, dtype: args.Dtype, hasBias: false);

        RegisterComponents();

        this.freqs_compex = Utils.PrecomputeThetaPosFrequencies(args.Dim / args.NHeads, args.MaxSeqLen * 2, args.Device);
    }

    public ModelArgs Args => this.args;

    public override Tensor forward(Tensor tokens, int startPos)
    {
        // (B, Seq_Len) -> (B, Seq_Len, Dim)
        var batch = tokens.shape[0];
        var seqLen = (int)tokens.shape[1];

        // print tokens shape
        Console.WriteLine($"tokens shape: {string.Join(",", tokens.shape)}");
        var h = this.tok_embeddings.forward(tokens);
        var freqsComplex = this.freqs_compex[startPos..(startPos + seqLen)];

        Tensor? mask = null;
        if (seqLen > 1)
        {
            var device = h.device;
            mask = torch.full(new long[] {seqLen, seqLen}, dtype: ScalarType.Float32, value: float.NegativeInfinity, device: device);
            // (B, Seq_Len) -> (B, Seq_Len, Seq_Len)
            mask = torch.triu(mask, diagonal: 1);
            // (B, Seq_Len, Seq_Len) -> (B, Seq_Len, Seq_Len)

            var zeros = torch.zeros(seqLen, startPos, device: device);
            mask = torch.hstack([zeros, mask]).type_as(h);
        }
        for (int i = 0; i < this.nLayers; i++)
        {
            h = this.layers[i].forward(h, startPos, freqsComplex, mask);
        }


        // (B, Seq_Len, Dim) -> (B, Seq_Len, Dim)
        h = this.norm.forward(h);
        // (B, Seq_Len, Dim) -> (B, Seq_Len, Vocab_Size)
        var output = this.output.forward(h);

        return output;
    }
}
