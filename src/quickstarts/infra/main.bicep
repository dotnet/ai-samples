targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for Resource Group and GPT Model')
@allowed(['australiaeast', 'canadaeast', 'francecentral', 'southindia', 'swedencentral', 'uksouth', 'westus'])
param gptLocation string

@minLength(1)
@description('The location used for Dall-e')
@allowed(['australiaeast', 'eastus', 'swedencentral'])
param dalleLocation string

@description('String representing the ID of the logged-in user')
param principalId string = ''

@description('Whether the deployment is running on GitHub Actions')
param runningOnGh string = ''
 
@description('Whether the deployment is running on Azure DevOps Pipeline')
param runningOnAdo string = ''

var tags = {
  'azd-env-name': environmentName
}

var abbrs = loadJsonContent('./abbreviations.json')

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: '${abbrs.resourcesResourceGroups}${environmentName}'
  location: gptLocation
  tags: tags
}

module gpt 'gpt.bicep' = {
  scope: rg
  name: 'aigpt'
  params: {
      location: gptLocation
      tags: tags
  }
}

module dalle 'dalle.bicep' = {
  scope: rg
  name: 'aidalle'
  params: {
      location: dalleLocation
      tags: tags
  }
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
      location: gptLocation
      tags: tags
      principalId: principalId
      runningOnAdo: runningOnAdo
      runningOnGh: runningOnGh
  }
}

output AZURE_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID

output AZURE_OPENAI_GPT_ENDPOINT string = gpt.outputs.AZURE_OPENAI_ENDPOINT
output AZURE_OPENAI_GPT_NAME string = gpt.outputs.AZURE_OPENAI_GPT_NAME
output AZURE_OPENAI_GPT_KEY string = gpt.outputs.AZURE_OPENAI_KEY

output AZURE_OPENAI_DALLE_ENDPOINT string = dalle.outputs.AZURE_OPENAI_ENDPOINT
output AZURE_OPENAI_DALLE_NAME string = dalle.outputs.AZURE_OPENAI_DALLE_NAME
output AZURE_OPENAI_DALLE_KEY string = dalle.outputs.AZURE_OPENAI_KEY
