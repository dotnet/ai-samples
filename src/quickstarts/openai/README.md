# Quickstarts

These samples demonstrates how to use OpenAI with `gpt-3.5-turbo` and `dall-e-3` models, from a simple .NET 8.0 console application. It consists of a simple console applications, running locally, that will send request to the OpenAI service. 

## Prerequisites

- .NET 8.0 SDK - [Install the .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Setup an [Account with OpenAI](https://platform.openai.com/docs/quickstart/account-setup)
- On Windows, PowerShell `v7+` is required. To validate your version, run `pwsh` in a terminal. It should returns the current version. If it returns an error, execute the following command: `dotnet tool update --global PowerShell`.

## Semantic Kernel (SK)

[Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/overview/) is an open-source library that lets you easily build generative AI solutions using .NET. As a highly extensible SDK, you can use Semantic Kernel with models from OpenAI, Azure OpenAI, Hugging Face, and more!

## Trying the samples

Once you have an [API key from OpenAI](https://platform.openai.com/docs/quickstart/account-setup), it's time to select one of our samples to experiment with different scenarios.

| Sample                | Description                         | Semantic Kernel | Azure OpenAI SDK |
|-----------------------|-------------                        |-----------------|------------------|
| Hike Benefits Summary | Summarize long text to a few words. | [SK](semantic-kernel/01-HikeBenefitsSummary/README.md) | [SDK](azure-openai-sdk/01-HikeBenefitsSummary/README.md) |
| Hiker AI              | Chat with the AI and get hike recommendation. | [SK](semantic-kernel/02-HikerAI/README.md) | [SDK](azure-openai-sdk/02-HikerAI/README.md) |
| Chatting About my Previous Hikes | Chat with the AI about your previous hikes. | [SK](semantic-kernel/03-ChattingAboutMyHikes/README.md) | [SDK](azure-openai-sdk/03-ChattingAboutMyHikes/README.md) |
| Hiker AI Pro          | Extending the AI model with a local function using Function Tool. | [SK](semantic-kernel/04-AiAndNative/README.md) | [SDK](azure-openai-sdk/04-HikerAIPro/README.md) |
| Hike Images           | Generate postal card images to invite your friends for a hike. | [SK](semantic-kernel/05-HikeImages/README.md) | [SDK](azure-openai-sdk/05-HikeImages/README.md) |