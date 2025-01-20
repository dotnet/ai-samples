// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Evaluation.Setup;

public class EnvironmentVariables
{
    private static readonly IDictionary<string, string> s_environmentVariableCache = new Dictionary<string, string>();

    private static string GetEnvironmentVariable(string variableName)
    {
        if (!s_environmentVariableCache.TryGetValue(variableName, out string? value))
        {
            value =
                Environment.GetEnvironmentVariable(variableName) ??
                throw new Exception($"Environment variable {variableName} not set.");

            s_environmentVariableCache[variableName] = value;
        }

        return value;
    }

    private static bool TryGetEnvironmentVariable(string variableName, [NotNullWhen(true)] out string? value)
    {
        if (!s_environmentVariableCache.TryGetValue(variableName, out value))
        {
            value = Environment.GetEnvironmentVariable(variableName);

            if (value is not null)
            {
                s_environmentVariableCache[variableName] = value;
            }
        }

        return value is not null;
    }

    private static bool TryGetInputTokenLimit(string variableName, [NotNullWhen(true)] out int? limit)
    {
        if (TryGetEnvironmentVariable(variableName, out string? value) && int.TryParse(value, out int valueInteger))
        {
            limit = valueInteger;
            return true;
        }
        else
        {
            limit = null;
            return false;
        }
    }

    #region Azure AI Inference
    public static string AzureAIInferenceEndpoint
        => GetEnvironmentVariable("EVAL_SAMPLE_AZURE_AI_INFERENCE_ENDPOINT");

    public static string AzureAIInferenceAPIKey
        => GetEnvironmentVariable("EVAL_SAMPLE_AZURE_AI_INFERENCE_API_KEY");

    public static string AzureAIInferenceModel
        => GetEnvironmentVariable("EVAL_SAMPLE_AZURE_AI_INFERENCE_MODEL");

    public static int? AzureAIInferenceModelInputTokenLimit =>
        TryGetInputTokenLimit("EVAL_SAMPLE_AZURE_AI_INFERENCE_MODEL_INPUT_TOKEN_LIMIT", out int? limit)
            ? limit
            : null;
    #endregion

    #region Azure OpenAI
    public static string AzureOpenAIEndpoint
        => GetEnvironmentVariable("EVAL_SAMPLE_AZURE_OPENAI_ENDPOINT");

    public static string AzureOpenAIModel
        => GetEnvironmentVariable("EVAL_SAMPLE_AZURE_OPENAI_MODEL");

    public static int? AzureOpenAIModelInputTokenLimit =>
        TryGetInputTokenLimit("EVAL_SAMPLE_AZURE_OPENAI_MODEL_INPUT_TOKEN_LIMIT", out int? limit)
            ? limit
            : null;
    #endregion

    #region Ollama
    public static string OllamaEndpoint
        => GetEnvironmentVariable("EVAL_SAMPLE_OLLAMA_ENDPOINT");

    public static string OllamaModel
        => GetEnvironmentVariable("EVAL_SAMPLE_OLLAMA_MODEL");

    public static int? OllamaModelInputTokenLimit =>
        TryGetInputTokenLimit("EVAL_SAMPLE_OLLAMA_MODEL_INPUT_TOKEN_LIMIT", out int? limit)
            ? limit
            : null;
    #endregion

    #region OpenAI
    public static string OpenAIAPIKey
        => GetEnvironmentVariable("EVAL_SAMPLE_OPENAI_API_KEY");

    public static string OpenAIModel
        => GetEnvironmentVariable("EVAL_SAMPLE_OPENAI_MODEL");

    public static int? OpenAIModelInputTokenLimit =>
        TryGetInputTokenLimit("EVAL_SAMPLE_OPENAI_MODEL_INPUT_TOKEN_LIMIT", out int? limit)
            ? limit
            : null;
    #endregion

    public static string StorageRootPath
    {
        get
        {
            string storageRootPath = GetEnvironmentVariable("EVAL_SAMPLE_STORAGE_ROOT_PATH");
            storageRootPath = Path.GetFullPath(storageRootPath);
            Directory.CreateDirectory(storageRootPath);
            return storageRootPath;
        }
    }
}
