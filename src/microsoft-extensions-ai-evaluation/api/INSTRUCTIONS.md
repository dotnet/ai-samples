# Microsoft.Extensions.AI.Evaluation - API Usage Examples - Instructions

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://visualstudio.microsoft.com/downloads/)

## Running the examples

The examples can be executed using the `dotnet test` command on the command line, or from within Visual Studio and
Visual Studio Code using the built-in test runners in each IDE.

The following setup steps are required to set up the LLM connection / endpoint that will be used for all examples
(i.e., for both the [Evaluation API Examples](./evaluation/README.md), as well as the
[Reporting API Examples](./reporting/README.md)).

1. **Decide which LLM provider you would like to use:** The examples are authored to run against Azure Open AI by
   default and have been tested against GPT-4o. You can easily switch to use Azure AI Inference, Ollama, or Open AI by
   changing one line of code within [`TestSetup.GetChatConfiguration()`](./evaluation/Setup/TestSetup.cs#L24).

2. **Configure environment variables that define the endpoint and model you would like to use:** Open
   [`EnvironmentVariables.cs`](./evaluation/Setup/EnvironmentVariables.cs) to figure out what environment variables you
   need to set depending on the LLM provider selected above. For example, if you selected Azure Open AI, you will need
   to set the following environment variables to specify the endpoint URL, and the name of the deployed model
    respectively: `EVAL_SAMPLE_AZURE_OPENAI_ENDPOINT` and `EVAL_SAMPLE_AZURE_OPENAI_MODEL`.

   **Note:** If the model or provider you selected enforces an input token limit, you can also set the corresponding
   `EVAL_SAMPLE_*_INPUT_TOKEN_LIMIT` environment variable to specify the limit. However, specifying token limit is
   strictly optional.

   **Note:** If you plan to run the example unit tests using Visual Studio or Visual Studio Code, the most convenient
   option may be to set the above environment variables globally for your user / machine. This will ensure that the
   environment variables are always available to your IDE's test runner (and to all test runner child processes)
   regardless of how the IDE was launched.

The following additional setup steps are required to run the [Reporting API Examples](./reporting/README.md).

3. **Set the `EVAL_SAMPLE_STORAGE_ROOT_PATH` environment variable**: This variable should point to a dedicated
   directory on your machine under which all the cached LLM responses, evaluation result data, and generated
   reports for the [Reporting API Examples](./reporting/README.md) will be saved. Again, it is recommended to set this
   variable globally for your user / machine when running the example unit tests from within the IDE.

4. The last couple of unit tests in the [Reporting API Examples](./reporting/README.md) project demonstrate how to
   programmatically generate a report containing the results for all evaluations performed in previously executed
   tests. Inspect this report to understand how the different concepts demonstrated in the examples are surfaced in the
   report.

   **Note:** The report generation tests above will produce a report that includes results from all included
   examples **only if you execute all unit tests in the project as part of a single execution**. In other words, the
   report generation examples above won't quite work if you execute the tests one at a time. This is because the code
   in the report generation examples only includes results for the the latest (current) execution in the generated
   report. When the examples are executed one by one, each result is considered part of a different (previous)
   execution, and the results from these older executions will not be included in the generated report.

## Generating reports using the `aieval` dotnet tool

You can also use the command line based `aieval` dotnet tool that ships as part of the
[Microsoft.Extensions.AI.Evaluation.Console](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Console)
NuGet package to generate and view reports. In fact, the recommended way to generate reports as part of your CI builds
is to run this tool in your CI/CD pipeline.

**Note:** The `aieval` dotnet tool only supports the disk-based result storage at the moment.

To generate a report using the `aieval` dotnet tool,

1. First run all the included examples as part of a single test run (either from the command line by running
   `dotnet test`, or from within the IDE).
   
2. Then execute the following command from under the directory that contains the current README.md file (i.e. from
   under `src/ai-samples/src/microsoft-extensions-ai-evaluation/api`). This will make the tool available for use on the
   command line under this directory.

   ```
   dotnet tool restore
   ```

   **Note**: It is a good idea to re-run the above command any time you pull down newer versions of this sample.

3. Then run the installed `aieval` tool using the following command after replacing `<EVAL_SAMPLE_STORAGE_ROOT_PATH>`
   with the path to the storage root directory that you specified in step 3 above.

   ```
   dotnet aieval report -p <EVAL_SAMPLE_STORAGE_ROOT_PATH> -o <%EVAL_SAMPLE_STORAGE_ROOT_PATH>/report.html
   ```

4. Now open the generated `report.html` file above in your browser to view the report.

   **Note**: The `aieval` tool also supports some options for cleaning up cached responses and stored evaluation
   results. Run `dotnet aieval --help` for more information.

## Installing the `aieval` dotnet tool in your own repository

For production usages outside the current sample, we recommend that you install the `aieval` tool under
your repository. To do this,

1. Run the following command from the directory in your repository where you wish to install the tool. Before running,
   remember to replace `<VERSION>` to match the version of the
   [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation)
   NuGet package referenced in your repository.

   ```
   dotnet tool install Microsoft.Extensions.AI.Evaluation.Console --version <VERSION> --create-manifest-if-needed
   ```

2. Commit the resulting `.config/dotnet-tools.json` file to your repository's source control. Other users will then be
   able to restore the tool by running the following command from the same directory.
   
   ```
   dotnet tool restore
   ```

3. Then as you update the versions of
   [Microsoft.Extensions.AI.Evaluation](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation) and related
   NuGet packages in your repository, you can also update the version of the `aieval` tool in the committed
   `.config/dotnet-tools.json` file to keep the installed tool and the referenced libraries in sync.
