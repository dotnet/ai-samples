using LLMEval.Core;
using Csv;

namespace LLMEval.Outputs;

public static class ExportToCsv
{
    public static void WriteCsv(LLMEvalResults results, string fileLocation)
    {
        var csv = CreateCsv(results);
        File.WriteAllText(fileLocation, csv);
    }

    public static string CreateCsv(LLMEvalResults results)
    {
        // headers
        var columnNames = new List<string> { "Input", "Output" };
        var first = results.EvalResults.First();
        foreach (var key in first.Results.Keys)
        {
            columnNames.Add(key);
        }

        // rows 
        var rows = new List<string[]>();
        foreach (var result in results.EvalResults)
        {
            var row = new List<string>
                {
                    result.Subject.Input,
                    result.Subject.Output
                };

            foreach (var value in result.Results.Values)
            {
                row.Add(value?.ToString() ?? string.Empty);
            }

            rows.Add(row.ToArray());
        }

        var csv = CsvWriter.WriteToText(columnNames.ToArray(), rows.ToArray(), ',');
        return csv;
    }
}
