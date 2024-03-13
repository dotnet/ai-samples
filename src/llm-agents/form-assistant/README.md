## Form Assistant

This example shows how to build an LLM form assistant to collect information in a polite and conversational manner.

### Prerequisites
- dotnet 8.0 or later
- Access to Azure OpenAI or OpenAI API

### Getting Started
- Clone the repository and cd into this directory
- Run the following command to start the application
```bash
dotnet run
```

The application will start and form assistant will start asking questions to collect information like name, email, phone number, etc. Once all the information is collected, it will display the collected information.

### Output
The output will look like this, where the form agent asks questions and collects information in a polite and conversational manner and user responds to the questions.

> [!NOTE]
> To automate the process, the user in the output below is also an LLM agent powered by gpt-3.5

![output](./img/output.gif)
