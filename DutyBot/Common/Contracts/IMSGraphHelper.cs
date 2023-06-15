using Microsoft.Graph.Models;

namespace DutyBot.Common.Contracts
{
    public interface IMSGraphHelper
    {
        Task<User> GetUserByNameAsync(string name);

        Task<User> GetUserByPrincipalNameAsync(string name);
    }
}