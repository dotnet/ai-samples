@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

@description('String representing the ID of the logged-in user')
param principalId string = ''

@description('Name of the key vault used by the app')
param keyvaultName string = ''

@description('Name of the openai key secret in the keyvault')
param openAIKeyName string = 'AZURE-OPEN-AI-KEY'

@description('Name of the openai key secret in the keyvault')
param openAIName string

@description('Whether the deployment is running on GitHub Actions')
param runningOnGh string = ''
 
@description('Whether the deployment is running on Azure DevOps Pipeline')
param runningOnAdo string = ''

var principalType = empty(runningOnGh) && empty(runningOnAdo) ? 'User': 'ServicePrincipal'

var resourceToken = uniqueString(resourceGroup().id)

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'mi-${resourceToken}'
  location: location
  tags: tags
}



// create secret to store openai api key
module openAIKey 'core/security/keyvault-secret.bicep' = {
    name: 'openai-key'
    params: {
        name: openAIKeyName
        keyVaultName: keyvaultName
        secretValue: listKeys(resourceId(subscription().subscriptionId, resourceGroup().name, 'Microsoft.CognitiveServices/accounts', openAIName), '2023-05-01').key1
    }
}

output MANAGED_IDENTITY_CLIENT_ID string = managedIdentity.properties.clientId

