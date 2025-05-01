// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Evaluation.Evaluators;
using Evaluation.Setup;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.AI.Evaluation.Safety;

namespace Reporting;

public partial class ReportingExamples
{
    /// For the below example, we use the same <see cref="s_safetyServiceConfiguration"/> as
    /// <see cref="Example08_RunningSafetyEvaluators()"/> to set up the connection parameters that the evaluators
    /// included as part of the Microsoft.Extensions.AI.Evaluation.Safety NuGet package (such as
    /// <see cref="HateAndUnfairnessEvaluator"/>, <see cref="ProtectedMaterialEvaluator"/>, etc.) need in order to
    /// communicate with the Azure AI Content Safety service.
    /// 
    /// However, when converting this <see cref="ContentSafetyServiceConfiguration"/> to a
    /// <see cref="ChatConfiguration"/>, we additionally include the same <see cref="s_chatConfiguration"/> that we use
    /// in other examples to set up the LLM connection for LLM-based evaluators. By including
    /// <see cref="s_chatConfiguration"/> as the 'originalChatConfiguration' in the call to
    /// <see cref="ContentSafetyServiceConfigurationExtensions.ToChatConfiguration(ContentSafetyServiceConfiguration, ChatConfiguration?)"/>
    /// below, we get back a <see cref="ChatConfiguration"/> that can be used both by LLM-based evaluators such as the
    /// ones included in the Microsoft.Extensions.AI.Evaluation.Quality NuGet package, as well as by the evaluators
    /// included in the Microsoft.Extensions.AI.Evaluation.Quality NuGet package.
    /// 
    /// Note that the response caching functionality is supported and works the same way regardless of whether the
    /// included evaluators talk to an LLM (as is the case for the evaluators that are part of the
    /// Microsoft.Extensions.AI.Evaluation.Quality NuGet package) or to the Azure Content Safety service (as is the
    /// case for the below evaluators that are part of the Microsoft.Extensions.AI.Evaluation.Safety NuGet package).

    private static readonly ReportingConfiguration? s_qualityAndSafetyReportingConfiguration =
        s_safetyServiceConfiguration is null
            ? null
            : DiskBasedReportingConfiguration.Create(
                storageRootPath: EnvironmentVariables.StorageRootPath,
                evaluators: GetQualityAndSafetyEvaluators(),
                chatConfiguration: s_safetyServiceConfiguration.ToChatConfiguration(originalChatConfiguration: s_chatConfiguration),
                enableResponseCaching: true,
                executionName: ExecutionName,
                tags: GetTags());

    [TestMethod]
    public async Task Example10_RunningQualityAndSafetyEvaluatorsTogether()
    {
        /// This test requires additional environment variables to be set in order to use the Azure AI Content Safety
        /// service. The test is skipped if these environment variables are not set.
        SkipTestIfSafetyEvaluatorsAreNotConfigured();

        await using ScenarioRun scenarioRun =
            await s_qualityAndSafetyReportingConfiguration.CreateScenarioRunAsync(
                this.ScenarioName,
                additionalTags: ["Mars", "Venus"]);

        (IList<ChatMessage> messages, ChatResponse modelResponse) =
            await GetAstronomyConversationAsync(
                chatClient: scenarioRun.ChatConfiguration!.ChatClient,
                astronomyQuestion: "How far is the Venus from the Mars at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);
    }

    private static IEnumerable<IEvaluator> GetQualityAndSafetyEvaluators()
    {
        if (s_safetyServiceConfiguration is null)
        {
            yield break;
        }

        // Quality evaluators.
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        yield return coherenceEvaluator;

        IEvaluator relevanceEvaluator = new RelevanceEvaluator();
        yield return relevanceEvaluator;

        IEvaluator measurementSystemEvaluator = new MeasurementSystemEvaluator();
        yield return measurementSystemEvaluator;

        IEvaluator wordCountEvaluator = new WordCountEvaluator();
        yield return wordCountEvaluator;

        // Safety evaluators.
        IEvaluator contentHarmEvaluator = new ContentHarmEvaluator();
        yield return contentHarmEvaluator;

        IEvaluator protectedMaterialEvaluator = new ProtectedMaterialEvaluator();
        yield return protectedMaterialEvaluator;

        IEvaluator indirectAttackEvaluator = new IndirectAttackEvaluator();
        yield return indirectAttackEvaluator;
    }
}
