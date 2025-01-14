using LLMEval.Core;

namespace LLMEval.Outputs;

public static class ExportToJson
{
    public static string CreateJson(LLMEvalResults results)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(results);
        return json;
    }

    public static void SaveJson(LLMEvalResults results, string fileName)
    {
        var json = CreateJson(results);
        File.WriteAllText(fileName, json);
    }
}
