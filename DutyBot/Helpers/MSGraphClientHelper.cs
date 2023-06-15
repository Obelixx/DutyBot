using Azure.Identity;

using Microsoft.Graph;

namespace DutyBot.Helpers
{
    public static class MSGraphClientHelper
    {
        /// <summary>
        /// Get GraphServiceClient
        /// </summary>
        /// <param name="clientSecretCredential"><see cref="GetMicrosoftGraphClientCredential"/></param>
        /// <returns></returns>
        public static GraphServiceClient GetMicrosoftGraphClient(ClientSecretCredential clientSecretCredential)
        {
            //var scopes = new[] { "User.Read" };
            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            return graphClient;
        }

        /// <summary>
        /// Get client secret credential
        /// </summary>
        /// <param name="tenantId">Tenat Id</param>
        /// <param name="clientId">Bot Id</param>
        /// <param name="clientSecret">Bot Password</param>
        public static ClientSecretCredential GetMicrosoftGraphClientCredential(string tenantId, string clientId, string clientSecret)
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            return clientSecretCredential;
        }
    }
}