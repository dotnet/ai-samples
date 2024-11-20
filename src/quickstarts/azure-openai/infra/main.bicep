targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
@allowed(['australiaeast'])
param location string

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
  location: location
  tags: tags
}

module ai 'ai.bicep' = {
  scope: rg
  name: 'ai'
  params: {
      location: location
      tags: tags
  }
}


module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
      location: location
      tags: tags
      principalId: principalId
      runningOnAdo: runningOnAdo
      runningOnGh: runningOnGh
  }
}

module openAiRoleUser 'core/security/role.bicep' ={
  scope: rg
  name: 'openai-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    principalType: 'User'
  }
}


output AZURE_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output AZURE_OPENAI_ENDPOINT string = ai.outputs.AZURE_OPENAI_ENDPOINT
output AZURE_OPENAI_GPT_NAME string = ai.outputs.AZURE_OPENAI_GPT_NAME
output AZURE_OPENAI_DALLE_NAME string = ai.outputs.AZURE_OPENAI_DALLE_NAME
