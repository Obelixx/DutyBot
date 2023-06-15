// Auto generated content, please customize files under provision folder

@secure()
param provisionParameters object
param provisionOutputs object
@secure()
param currentAppSettings object

var webAppName = split(provisionOutputs.azureWebAppBotOutput.value.resourceId, '/')[8]
var webappEndpoint = provisionOutputs.azureWebAppBotOutput.value.siteEndpoint
var m365ClientId = provisionParameters['m365ClientId']
var m365ClientSecret = provisionParameters['m365ClientSecret']
var m365TenantId = provisionParameters['m365TenantId']
var m365OauthAuthorityHost = provisionParameters['m365OauthAuthorityHost']
var botId = provisionParameters['botAadAppClientId']
var m365ApplicationIdUri = 'api://botid-${botId}'
var botAadAppClientId = provisionParameters['botAadAppClientId']
var botAadAppClientSecret = provisionParameters['botAadAppClientSecret']

resource webAppSettings 'Microsoft.Web/sites/config@2021-02-01' = {
  name: '${webAppName}/appsettings'
  properties: union({
    TeamsFx__Authentication__ClientId: m365ClientId // Client id of AAD application
    TeamsFx__Authentication__ClientSecret: m365ClientSecret // Client secret of AAD application
    TeamsFx__Authentication__OAuthAuthority: uri(m365OauthAuthorityHost, m365TenantId) // AAD authority host
    TeamsFx__Authentication__Bot__InitiateLoginEndpoint: uri(provisionOutputs.azureWebAppBotOutput.value.siteEndpoint, 'bot-auth-start') // The page is used to let users consent required OAuth permissions during bot SSO process
    TeamsFx__Authentication__ApplicationIdUri: m365ApplicationIdUri // Application ID URI of AAD application
    BOT_ID: botAadAppClientId // ID of your bot
    BOT_PASSWORD: botAadAppClientSecret // Secret of your bot
    IDENTITY_ID: provisionOutputs.identityOutput.value.identityClientId // User assigned identity id, the identity is used to access other Azure resources
  }, currentAppSettings)
}