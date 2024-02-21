@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

@description('String representing the ID of the logged-in user ')
param principalId string = ''

var resourceToken = uniqueString(resourceGroup().id)

// create a keyvault to store openai secrets
module keyvault 'core/security/keyvault.bicep' = {
    name: 'kv${resourceToken}'
    scope: resourceGroup()
    params: {
        name: 'kv${resourceToken}'
        location: location
        tags: tags
        principalId: principalId
    }
}

output AZURE_KEY_VAULT_ENDPOINT string = keyvault.outputs.endpoint
output AZURE_KEY_VAULT_NAME string = keyvault.name
