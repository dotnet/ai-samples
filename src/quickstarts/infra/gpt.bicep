@description('The location used for all deployed resources')
param location string = ''

@description('Tags that will be applied to all resources')
param tags object = {}

var resourceToken = uniqueString(resourceGroup().id)

var abbrs = loadJsonContent('./abbreviations.json')

// the openai deployments to create
var gptDeployment = [
  {
    name: 'gpt35${resourceToken}'
    sku: {
      name: 'Standard'
      capacity: 2
    }
    model: {
      format: 'OpenAI'
      name: 'gpt-35-turbo'
      version: '1106'
    }
  }
]

// create the openai resources
module gptOpenAi './core/ai/cognitiveservices.bicep' = {
  name: 'gptopenai'
  scope: resourceGroup()
  params: {
    name: '${abbrs.cognitiveServicesAccounts}-gpt${resourceToken}'
    location: location
    tags: tags
    deployments: gptDeployment
  }
}

output AZURE_OPENAI_ENDPOINT string = gptOpenAi.outputs.endpoint
output AZURE_OPENAI_GPT_NAME string = 'gpt35${resourceToken}'
output AZURE_OPENAI_NAME string = 'aigpt${resourceToken}'
output AZURE_OPENAI_KEY string = gptOpenAi.outputs.key1
