@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

@description('Name of the openai key secret in the keyvault')
param openAIKeyName string = 'AZURE-OPEN-AI-KEY'

var resourceToken = uniqueString(resourceGroup().id)

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
      version: '0613'
    }
  }
]

// create the openai resources
module openAi './core/ai/cognitiveservices.bicep' = {
  name: 'openai'
  scope: resourceGroup()
  params: {
    name: 'ai${resourceToken}'
    location: location
    tags: tags
    deployments: openaiDeployment
  }
}

output AZURE_OPENAI_ENDPOINT string = openAi.outputs.endpoint
output AZURE_OPENAI_GPT_NAME string = 'gpt35${resourceToken}'
output AZURE_OPENAI_KEY_NAME string = openAIKeyName
output AZURE_OPENAI_NAME string = 'ai${resourceToken}'
output AZURE_OPENAI_TEXT_EMBEDDING_NAME string = 'text${resourceToken}'
