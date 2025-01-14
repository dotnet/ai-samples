#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using LLMEval.Data;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace UserStoryGenerator;

public class UserStorySkill
{
    private readonly KernelFunction _createUserStoryFunction;

    private readonly Kernel _kernel;

    public static UserStorySkill Create(Kernel kernel)
    {

        string promptTemplate = EmbeddedResource.Read("_prompts.userstoryclassic.skprompt.txt")!;

        return new UserStorySkill(kernel, kernel.CreateFunctionFromPrompt(promptTemplate));
    }

    public UserStorySkill(Kernel kernel, KernelFunction promptFunction)
    {
        _createUserStoryFunction = promptFunction;
        _kernel = kernel;
    }

    public async Task<UserStory> GetUserStory(string description, string? projectContext = null, string? personaName = null)
    {
        var context = new KernelArguments
        {
            { "ProjectContext", projectContext ?? "software development project" },
            { "ContextTopic", description! },
            { "Persona", personaName ?? "software engineer" }
        };

        var result = await _createUserStoryFunction.InvokeAsync(_kernel, context);
        var resultString = result.ToString();

        var userStory = new UserStory();
        try
        {
            userStory = JsonSerializer.Deserialize<UserStory>(resultString, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (JsonException jsonExc)
        {
            userStory.Persona = "An error occurred while generating the user story. Error descripton";
            userStory.Description = jsonExc.Message.ToString();
        }

        return userStory;
    }
}