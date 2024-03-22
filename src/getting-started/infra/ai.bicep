@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

var resourceToken = uniqueString(resourceGroup().id)

var abbrs = loadJsonContent('./abbreviations.json')

// the openai deployments to create
var openaiDeployment = [
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
  },{
    name: 'dal3${resourceToken}'
    sku: {
      name: 'Standard'
      capacity: 1
    }
    model: {
      format: 'OpenAI'
      name: 'dall-e-3'
    }
  }

]

// create the openai resources
module openAi './core/ai/cognitiveservices.bicep' = {
  name: 'openai'
  scope: resourceGroup()
  params: {
    name: '${abbrs.cognitiveServicesAccounts}${resourceToken}'
    location: location
    tags: tags
    deployments: openaiDeployment
  }
}

output AZURE_OPENAI_ENDPOINT string = openAi.outputs.endpoint
output AZURE_OPENAI_GPT_NAME string = 'gpt35${resourceToken}'
output AZURE_OPENAI_DALLE_NAME string = 'dal3${resourceToken}'
output AZURE_OPENAI_NAME string = 'ai${resourceToken}'
output AZURE_OPENAI_TEXT_EMBEDDING_NAME string = 'text${resourceToken}'
output AZURE_OPENAI_KEY string = openAi.outputs.key1
