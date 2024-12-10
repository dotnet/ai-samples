// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Policy;
using Evaluation.Setup;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Formats.Html;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage.Disk;

namespace Reporting;

public partial class ReportingExamples
{
    [TestMethod]
    public async Task Example10_GeneratingReportProgrammatically()
    {
        /// This example demonstrates how to generate an evaluation report programmatically using the results stored in
        /// the disk-based result store present under the directory that you specified via the
        /// 'EVAL_SAMPLE_STORAGE_ROOT_PATH' environment variable.

        var results = new List<ScenarioRunResult>();
        IResultStore resultStore = new DiskBasedResultStore(EnvironmentVariables.StorageRootPath);

        /// Use the <see cref="resultStore"/> object above to read all results for the 'latest' execution.
        await foreach (string executionName in resultStore.GetLatestExecutionNamesAsync(count: 1))
        {
            await foreach (ScenarioRunResult result in resultStore.ReadResultsAsync(ExecutionName))
            {
                results.Add(result);
            }
        }

        string reportFilePath = Path.Combine(EnvironmentVariables.StorageRootPath, "report.html");
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

        /// Note that as described in the README.md file for this project, you can also generate the same report using
        /// the 'aieval' dotnet tool from the command line.
    }
}
