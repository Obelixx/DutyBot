using DutyBot.Common.Contracts;
using DutyBot.Helpers;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace DutyBot.BotActivityHandlers
{
    public class MessageActivityWithWhoIs : IMessageActivityHelper
    {
        private readonly IMSGraphHelper msGraph;

        public MessageActivityWithWhoIs(IMSGraphHelper msGraph)
        {
            this.msGraph = msGraph;
        }

        public async Task HandleMessageTurnContext(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            // testing search by AAD username
            if (turnContext.Activity.Text.StartsWith("who is "))
            {
                var term = turnContext.Activity.Text.Trim();
                term = term.Split("who is ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
                if (term != null)
                {
                    var user = await msGraph.GetUserByPrincipalNameAsync(term);
                    if (user != null)
                    {
                        var ca = new ChannelAccount(user.Id, user.DisplayName, "user", user.SecurityIdentifier);
                        var reply = BotMessageHelper.CreateMessageWithMention(ca, user.DisplayName, "User found: {0}");
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                    }
                }
            }
        }
    }
}
