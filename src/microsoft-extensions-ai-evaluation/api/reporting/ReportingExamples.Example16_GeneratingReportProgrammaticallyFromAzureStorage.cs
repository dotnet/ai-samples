// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Evaluation.Setup;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Formats.Html;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

namespace Reporting;

public partial class ReportingExamples
{
    [TestMethod]
    public async Task Example16_GeneratingReportProgrammaticallyFromAzureStorage()
    {
        SkipTestIfAzureStorageNotConfigured();

        /// This example demonstrates how to generate an evaluation report programmatically using the results stored in
        /// the Azure Storage result store that stores results for the examples present in
        /// <see cref="Example11_UsingAzureStorage_01"/> and <see cref="Example12_UsingAzureStorage_02"/>.

        var results = new List<ScenarioRunResult>();
        IEvaluationResultStore resultStore = new AzureStorageResultStore(s_dataLakeDirectoryClient);

        /// Use the <see cref="resultStore"/> object above to read all results for the 'latest' execution.
        await foreach (string executionName in resultStore.GetLatestExecutionNamesAsync(count: 1))
        {
            await foreach (ScenarioRunResult result in resultStore.ReadResultsAsync(executionName))
            {
                results.Add(result);
            }
        }

        string reportFilePath = Path.Combine(EnvironmentVariables.StorageRootPath, "report_azure_storage.html");
        IEvaluationReportWriter reportWriter = new HtmlReportWriter(reportFilePath);

        /// Generate a report containing the results read from the <see cref="resultStore"/> above.
        await reportWriter.WriteReportAsync(results);

        /// Open the generated report in the default browser.
        Process.Start(
            new ProcessStartInfo()
            {
                FileName = reportFilePath,
                UseShellExecute = true
            });

        /// Note that the above report generation code won't quite work if you execute the tests in this project one at
        /// a time (since each such result will then be considered part of a different (previous) execution as opposed
        /// to the current (latest) execution in this case). In other words, the report generation code above will only
        /// work when you run all unit tests in this project as part of a single execution.

        /// Note that as described in the INSTRUCTIONS.md file, you can also generate the same report using the
        /// 'aieval' dotnet tool from the command line.
    }
}
