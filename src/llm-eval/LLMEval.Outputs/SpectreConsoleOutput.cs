#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052

using LLMEval.Core;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;

namespace LLMEval.Output;

public static class SpectreConsoleOutput
{
    public static void DisplayTitle(string title = ".NET - LLM Eval")
    {
        AnsiConsole.Write(new FigletText(title).Centered().Color(Color.Purple));
    }

    public static void DisplayTitleH2(string subtitle)
    {
        AnsiConsole.MarkupLine($"[bold][blue]=== {subtitle} ===[/][/]");
        AnsiConsole.MarkupLine($"");
    }

    public static void DisplayTitleH3(string subtitle)
    {
        AnsiConsole.MarkupLine($"[bold]>> {subtitle}[/]");
        AnsiConsole.MarkupLine($"");
    }

    public static void DisplayJson(string json, string title = "", bool usePanel = false)
    {
        var jsonSpectre = new JsonText(json);

        var jsonPanel = new Panel(jsonSpectre);
        if (usePanel)
        {
            jsonPanel.Header = new PanelHeader(title);
        }
        AnsiConsole.Write(jsonPanel);
    }

    public static List<string> GetMenuOptions()
    {
        var list = new List<string> {
            "1 generated QA using LLM",
            "2 harcoded QAs",
            "1 harcoded User Story",
            "List of User Stories from a file",
            "List of QAs from a file",
            "List of QAs generated using a LLM",
            "Generate QAs associated to a topic, and export to json",
            "Type topic to generate a QA using LLM",
            "Type the QA to be tested"};
        return list;
    }

    public static List<string> SelectScenarios()
    {
        // Ask for the user's favorite fruits
        var scenarios = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select the [green].NET LLM Eval scenarios[/] to run?")
                .PageSize(10)
                .Required(true)
                .MoreChoicesText("[grey](Move up and down to reveal more scenarios)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a scenario, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(GetMenuOptions()));
        return scenarios;
    }

    public static int AskForNumber(string question)
    {
        var number = AnsiConsole.Ask<int>(@$"[green]{question}[/]");
        return number;
    }

    public static string AskForString(string question)
    {
        var response = AnsiConsole.Ask<string>(@$"[green]{question}[/]");
        return response;
    }

    public static void DisplayKernels(Kernel testKernel, Kernel evalKernel, Kernel genKernel)
    {
        // Create a table
        var table = new Table();

        // Add columns
        table.AddColumn("kernel name");
        table.AddColumn("service");
        table.AddColumn("Key - Value");

        DisplayKernelInfo(testKernel, "Test", table);
        DisplayKernelInfo(evalKernel, "Eval", table);
        DisplayKernelInfo(genKernel, "Gen", table);

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    public static void DisplayKernelInfo(Kernel kernel, string kernelName, Table table)
    {
        foreach (var service in kernel.GetAllServices<IChatCompletionService>().ToList())
        {
            AddRow(table, kernelName, "IChatCompletionService", service.Attributes);
        }

        foreach (var service in kernel.GetAllServices<ITextEmbeddingGenerationService>().ToList())
        {
            AddRow(table, kernelName, "ITextEmbeddingGenerationService", service.Attributes);
        }

        foreach (var service in kernel.GetAllServices<ITextGenerationService>().ToList())
        {
            AddRow(table, kernelName, "ITextGenerationService", service.Attributes);
        }
    }

    private static void AddRow(Table table, string kernelName, string serviceName, IReadOnlyDictionary<string, object?> services)
    {
        foreach (var atr in services)
        {
            List<Renderable> row = [new Markup($"[bold]< {kernelName} >[/]"), new Text(serviceName), new Text($"{atr.Key} - {atr.Value}")];
            table.AddRow(row.ToArray());
        }
    }

    public static void DisplayResults(LLMEvalResults results)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        DisplayTitleH3(results.EvalRunName);
#pragma warning restore CS8604 // Possible null reference argument.

        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Input");
        table.AddColumn("Output");
        var first = results.EvalResults.First();
        foreach (var key in first.Results.Keys)
        {
            table.AddColumn(new TableColumn(key).Centered());
        }
        table.Columns[0].PadLeft(1).PadRight(1);
        table.Columns[1].PadLeft(1).PadRight(1);

        foreach (var result in results.EvalResults)
        {
            List<Renderable> row = [
                new Text(result.Subject.Input),
                new Text(result.Subject.Output)];

            // add the evaluation results
            foreach (var value in result.Results.Values)
            {
                var intValue = (int)value;
                string color = "white";
                if (intValue <= 1)
                {
                    color = "red";
                }
                else if (intValue <= 3)
                {
                    color = "yellow";
                }
                else if (intValue <= 5)
                {
                    color = "green";
                }

                row.Add(new Markup($"[{color}]{value?.ToString() ?? string.Empty}[/]"));
            }
            table.AddRow(row.ToArray());
        }

        //Render the table to the console
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }
}
