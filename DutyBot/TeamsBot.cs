using DutyBot.BotActivityHandlers;
using DutyBot.Common.Contracts;
using DutyBot.Helpers;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;

using System.Xml;

namespace DutyBot
{
    /// <summary>
    /// An empty bot handler.
    /// You can add your customization code here to extend your bot logic if needed.
    /// </summary>
    public class TeamsBot : TeamsActivityHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IMSGraphHelper msGraph;

        public TeamsBot(IServiceProvider serviceProvider,IMSGraphHelper msGraph)
        {
            this.serviceProvider = serviceProvider;
            this.msGraph = msGraph;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type.Contains(ActivityTypes.Message))
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

                            //await turnContext.SendActivityAsync($"User found: {user.DisplayName} {user.UserPrincipalName} {user.Id}", cancellationToken: cancellationToken);
                        }
                    }
                }

                var handlers = serviceProvider.GetServices<IMessageActivityHelper>();
                foreach (var handler in handlers)
                {
                    handler.HandleMessageTurnContext(turnContext, cancellationToken);
                }
            }
        }

        
    }
}