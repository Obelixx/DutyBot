using DutyBot.Common.Contracts;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;

namespace DutyBot
{
    /// <summary>
    /// An empty bot handler.
    /// You can add your customization code here to extend your bot logic if needed.
    /// </summary>
    public class TeamsBot : TeamsActivityHandler
    {
        private readonly IServiceProvider serviceProvider;

        public TeamsBot(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type.Contains(ActivityTypes.Message))
            {
                var handlers = serviceProvider.GetServices<IMessageActivityHelper>();
                foreach (var handler in handlers)
                {
                    await handler.HandleMessageTurnContext(turnContext, cancellationToken);
                }
            }
        }
    }
}