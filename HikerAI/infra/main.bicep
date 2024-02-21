targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('String representing the ID of the logged-in user')
param principalId string = ''

@description('Name of the openai key secret in the keyvault')
param openAIKeyName string = 'AZURE-OPEN-AI-KEY'

@description('Whether the deployment is running on GitHub Actions')
param runningOnGh string = ''
 
@description('Whether the deployment is running on Azure DevOps Pipeline')
param runningOnAdo string = ''

var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module ai 'ai.bicep' = {
  scope: rg
  name: 'ai'
  params: {
      location: location
      tags: tags
      openAIKeyName: openAIKeyName
  }
}

module keyvault 'keyvault.bicep' = {
  scope: rg
  name: 'keyvault'
  params: {
      location: location
      tags: tags
      principalId: principalId
  }
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
      location: location
      tags: tags
      principalId: principalId
      keyvaultName: keyvault.outputs.AZURE_KEY_VAULT_NAME
      openAIKeyName: openAIKeyName
      openAIName: ai.outputs.AZURE_OPENAI_NAME
      runningOnAdo: runningOnAdo
      runningOnGh: runningOnGh
  }
}

output AZURE_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output AZURE_KEY_VAULT_ENDPOINT string = keyvault.outputs.AZURE_KEY_VAULT_ENDPOINT
output AZURE_OPENAI_KEY_NAME string = ai.outputs.AZURE_OPENAI_KEY_NAME
output AZURE_OPENAI_ENDPOINT string = ai.outputs.AZURE_OPENAI_ENDPOINT
output AZURE_OPENAI_GPT_NAME string = ai.outputs.AZURE_OPENAI_GPT_NAME