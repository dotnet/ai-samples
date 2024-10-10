#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace LLMEval.Test;

internal static class KernelFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="Kernel"/> class for testing purposes.
    /// </summary>
    /// <returns>A new instance of the <see cref="Kernel"/> class.</returns>
    public static Kernel CreatKernelTest()
    {
        // // sample to test a local model 
        // var builder = Kernel.CreateBuilder();
        // builder.AddOpenAIChatCompletion(
        //     modelId: "phi3",
        //     endpoint: new Uri("http://localhost:11434"),
        //     apiKey: "api");
        // return builder.Build();

        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var builder = Kernel.CreateBuilder();

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
        builder.AddAzureOpenAIChatCompletion(
            config["AZURE_OPENAI_MODEL"],
            config["AZURE_OPENAI_ENDPOINT"],
            config["AZURE_OPENAI_KEY"]);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.

        return builder.Build();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Kernel"/> class for evaluation purposes.
    /// </summary>
    /// <returns>A new instance of the <see cref="Kernel"/> class.</returns>
    public static Kernel CreateKernelEval()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var builder = Kernel.CreateBuilder();

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
        builder.AddAzureOpenAIChatCompletion(
            config["AZURE_OPENAI_MODEL"],
            config["AZURE_OPENAI_ENDPOINT"],
            config["AZURE_OPENAI_KEY"]);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.

        return builder.Build();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Kernel"/> class with the necessary configuration for generating data.
    /// </summary>
    /// <returns>A new instance of the <see cref="Kernel"/> class.</returns>
    public static Kernel CreateKernelGenerateData()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var builder = Kernel.CreateBuilder();
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
        builder.AddAzureOpenAIChatCompletion(
            config["AZURE_OPENAI_MODEL"],
            config["AZURE_OPENAI_ENDPOINT"],
            config["AZURE_OPENAI_KEY"]);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.

        return builder.Build();
    }
}
