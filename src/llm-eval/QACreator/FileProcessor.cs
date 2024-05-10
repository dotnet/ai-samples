using System.Text.Json;
using LLMEval.Data;

namespace QACreator;

public static class FileProcessor
{
    public static async Task<List<QA>> ProcessQAsInputFile(string fileName)
    {
        string fileContent = await File.ReadAllTextAsync(fileName);
        var results = JsonSerializer.Deserialize<List<QA>>(fileContent);

#pragma warning disable CS8603 // Possible null reference return.
        return results;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
