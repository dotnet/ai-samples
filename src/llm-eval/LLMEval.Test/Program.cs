﻿using Microsoft.SemanticKernel;
using LLMEval.Core;
using LLMEval.Data;
using LLMEval.Output;
using LLMEval.Test;
using QAGenerator;
using System.Text.Json;

namespace LLMEval;

class Program
{
    static async Task Main()
    {
        SpectreConsoleOutput.DisplayTitle();

        // ========================================
        // create kernels
        // ========================================
        SpectreConsoleOutput.DisplayTitleH2($"LLM Kernels");
        var kernelEval = KernelFactory.CreateKernelEval();
        var kernelTest = KernelFactory.CreateKernelEval();
        var kernelGen = KernelFactory.CreateKernelEval();
        SpectreConsoleOutput.DisplayKernels(kernelTest, kernelEval, kernelGen);

        // ========================================
        // create LLMEval and add evaluators
        // ========================================
        var kernelEvalFunctions = kernelEval.CreatePluginFromPromptDirectory("Prompts");
        var batchEval = new Core.LLMEval();

        batchEval
            .AddEvaluator(new PromptScoreEval("coherence", kernelEval, kernelEvalFunctions["coherence"]))
            .AddEvaluator(new PromptScoreEval("groundedness", kernelEval, kernelEvalFunctions["groundedness"]))
            .AddEvaluator(new PromptScoreEval("relevance", kernelEval, kernelEvalFunctions["relevance"]))
            .AddEvaluator(new LenghtEval());
        batchEval.SetMeterId("llama3");

        var scenarios = SpectreConsoleOutput.SelectScenarios();

        Console.WriteLine("");
        SpectreConsoleOutput.DisplayTitleH2($"Processing user selection");

        if (scenarios.Contains("1 generated QA using LLM"))
        {
            // ========================================
            // evaluate a random generated Question and Answer
            // ========================================
            var qa = await QALLMGenerator.GenerateQA(kernelGen);
            var qaProcessor = new QACreator.QACreator(kernelTest);
            var processResult = await qaProcessor.Process(qa);
            var results = await batchEval.ProcessSingle(processResult);
            results.EvalRunName = "Auto generated QA using LLM";
            SpectreConsoleOutput.DisplayResults(results);
        }

        if (scenarios.Contains("Type topic to generate a QA using LLM"))
        {
            // ========================================
            // evaluate a generated Question and Answer from a topic
            // ========================================

            // ask for the topic to generate the QAs
            var topic = SpectreConsoleOutput.AskForString("Type the topic to generate the QA?");

            var qa = await QALLMGenerator.GenerateQA(kernelGen, topic);
            var json = JsonSerializer.Serialize(qa, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            SpectreConsoleOutput.DisplayJson(json, "Generated QA using LLM", true);


            var qaProcessor = new QACreator.QACreator(kernelTest);
            var processResult = await qaProcessor.Process(qa);
            var results = await batchEval.ProcessSingle(processResult);
            results.EvalRunName = "Generated Question and Answer from a topic";
            SpectreConsoleOutput.DisplayResults(results);
        }

        if (scenarios.Contains("Type the QA to be tested"))
        {
            // ========================================
            // evaluate a Question and Answer from the user
            // ========================================

            // ask for the topic to generate the QAs
            var question = SpectreConsoleOutput.AskForString("Type the question:");
            var answer = SpectreConsoleOutput.AskForString("Type the answer:");
            var qa = new QA
            {
                Question = question,
                Answer = answer,
                Topic = ""
            };

            var qaProcessor = new QACreator.QACreator(kernelTest);
            var processResult = await qaProcessor.Process(qa);
            var results = await batchEval.ProcessSingle(processResult);
            results.EvalRunName = "Generated Question and Answer from a topic";
            SpectreConsoleOutput.DisplayResults(results);
        }

        if (scenarios.Contains("2 harcoded QAs"))
        {
            // ========================================
            // evaluate 2 Question and Answer
            // ========================================
            var qaProcessor = new QACreator.QACreator(kernelTest);
            var qa = new QA
            {
                Question = "two plus two",
                Answer = "'4' or 'four'",
                Topic = "Math"
            };

            var processResult = await qaProcessor.Process(qa);
            var results = await batchEval.ProcessSingle(processResult);
            results.EvalRunName = "Harcoded QA 1";
            SpectreConsoleOutput.DisplayResults(results);

            qa = new Data.QA
            {
                Question = "How do you suggest to crack an egg? Suggest the most common way to do this.",
                Answer = "Tap the egg on a flat surface and then crack the shell",
                Topic = "Cooking"
            };
            processResult = await qaProcessor.Process(qa);
            results = await batchEval.ProcessSingle(processResult);
            results.EvalRunName = "Harcoded QA 2";
            SpectreConsoleOutput.DisplayResults(results);
        }

        if (scenarios.Contains("1 harcoded User Story"))
        {
            // ========================================
            // evaluate a single User Story
            // ========================================
            var userstoryProcessor = new UserStoryCreator.UserStoryCreator(kernelTest);
            var userInput = new UserStory
            {
                Description = "Fix a broken appliance",
                ProjectContext = "At home",
                Persona = "Homeowner"
            };
            var processResult = await userstoryProcessor.Process(userInput);
            var results = await batchEval.ProcessSingle(processResult);
            results.EvalRunName = "Harcoded User Story Run 1";
            SpectreConsoleOutput.DisplayResults(results);
        }

        if (scenarios.Contains("List of User Stories from a file"))
        {
            // ========================================
            // evaluate a batch of inputs for User Stories from a file
            // ========================================
            SpectreConsoleOutput.DisplayTitleH2("Processing batch of User Stories");
            var fileName = "assets/data-15.json";
            Console.WriteLine($"Processing {fileName} ...");
            Console.WriteLine("");

            // load the sample data
            var userStoryCreator = new UserStoryCreator.UserStoryCreator(kernelTest);
            var userInputCollection = await UserStoryGenerator.FileProcessor.ProcessUserInputFile(fileName);

            var modelOutputCollection = await userStoryCreator.ProcessCollection(userInputCollection);
            var results = await batchEval.ProcessCollection(modelOutputCollection);
            results.EvalRunName = "User Story collection from file";
            SpectreConsoleOutput.DisplayResults(results);

            // convert results to json, save the results and display them in the console
            //var json = LLMEval.Outputs.ExportToJson.CreateJson(results);
            //LLMEval.Outputs.ExportToJson.SaveJson(results, "results.json");
            //SpectreConsoleOutput.DisplayJson(json, "User Story collection from file", true);

        }

        if (scenarios.Contains("List of QAs generated using a LLM"))
        {
            // ========================================
            // evaluate a batch of generated QAs generated using llm
            // ========================================
            SpectreConsoleOutput.DisplayTitleH2("Processing LLM generated QAs");

            // ask for the number of QAs to generate
            var numberOfQAs = SpectreConsoleOutput.AskForNumber("How many QAs do you want to generate?");

            // generate a collection of QAs using llms
            var llmGenQAs = await QALLMGenerator.GenerateQACollection(kernelGen, numberOfQAs);

            //// convert llmGenQAs to json
            //var json = JsonSerializer.Serialize(llmGenQAs, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //});
            //SpectreConsoleOutput.DisplayJson(json, "Generated QAs using LLM", true);


            var qaProcessor = new QACreator.QACreator(kernelTest);
            var modelOutputCollection = await qaProcessor.ProcessCollection(llmGenQAs);
            var results = await batchEval.ProcessCollection(modelOutputCollection);
            results.EvalRunName = "LLM generated QAs";
            SpectreConsoleOutput.DisplayResults(results);
        }

        // complete        
        SpectreConsoleOutput.DisplayTitleH2("Complete.");

    }
}
