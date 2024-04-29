#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

using UserStoryGenerator;
using LLMEval.Core;
using Microsoft.SemanticKernel;
using LLMEval.Data;

namespace UserStoryCreator;

public class UserStoryCreator : IInputProcessor
{
    private readonly UserStorySkill userStoryGenerator;

    public UserStoryCreator(Kernel kernel)
    {
        userStoryGenerator = UserStorySkill.Create(kernel);
    }

    public async Task<List<ModelOutput>> ProcessCollection<T>(T collection)
    {
        var result = new List<ModelOutput>();
        foreach (var userInput in collection as List<UserStory>)
        {
            var modelOutput = await Process(userInput);
            result.Add(modelOutput);
        }
        return result;
    }

    public async Task<ModelOutput> Process<T>(T source)
    {
        var userInput = source as UserStory;

        var userStory = await userStoryGenerator.GetUserStory(
            userInput.Description,
            userInput.ProjectContext,
            userInput.Persona);

        return new ModelOutput()
        {
            Input = @$"Generate a user story for persona: ""{userInput.Persona}"" so it can ""{userInput.Description}""",
            Output = $"{userStory!.Persona} - {userStory!.Description}"
        };
    }
}