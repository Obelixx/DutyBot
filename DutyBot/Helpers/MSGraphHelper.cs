using DutyBot.Common.Contracts;

using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace DutyBot.Helpers
{
    public class MSGraphHelper : IMSGraphHelper
    {
        private readonly GraphServiceClient client;

        public MSGraphHelper(GraphServiceClient client)
        {
            this.client = client;
        }

        public async Task<User> GetUserByPrincipalNameAsync(string name)
        {
            var userSearchResult = await client.Users.GetAsync((req) =>
            {
                //req.QueryParameters.Select = new string[] { "displayName", "userPrincipalName", "id" };
                req.QueryParameters.Filter = $"startsWith(userPrincipalName,'{name}@')";
            });

            if (userSearchResult == null || userSearchResult.Value == null || userSearchResult.Value.Count == 0)
            {
                return null;
            }

            return userSearchResult.Value.FirstOrDefault();
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            var userSearchResult = await client.Users.GetAsync((req) =>
            {
                //req.QueryParameters.Select = new string[] { "displayName", "userPrincipalName", "id" };
                req.QueryParameters.Filter = $"name eq {name}')";
            });

            if (userSearchResult == null || userSearchResult.Value == null || userSearchResult.Value.Count == 0)
            {
                return null;
            }

            return userSearchResult.Value.FirstOrDefault();
        }
    }
}
