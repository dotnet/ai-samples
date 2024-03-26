@description('The location used for all deployed resources')
param location string = ''

@description('Tags that will be applied to all resources')
param tags object = {}

var resourceToken = uniqueString(resourceGroup().id)

var abbrs = loadJsonContent('./abbreviations.json')

// the openai deployments to create
var dalleDeployment = [
  {
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
module dalleOpenAi './core/ai/cognitiveservices.bicep' = {
  name: 'dalleopenai'
  scope: resourceGroup()
  params: {
    name: '${abbrs.cognitiveServicesAccounts}-dalle-${resourceToken}'
    location: location
    tags: tags
    deployments: dalleDeployment
  }
}

output AZURE_OPENAI_GPT_ENDPOINT string = dalleOpenAi.outputs.endpoint
output AZURE_OPENAI_DALLE_NAME string = dalleDeployment.name
output AZURE_OPENAI_GPT_KEY string = dalleOpenAi.outputs.key1
