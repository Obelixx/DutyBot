using DutyBot.Common.Contracts;
using DutyBot.Helpers;

using Microsoft.Bot.Builder;

namespace DutyBot.BotActivityHandlers
{
    public class MessageActivityWithDuty : IMessageActivityHelper
    {
        public readonly string[] DutyKeywords = { "duty", "duties", "on air" };

        public async Task HandleMessageTurnContext(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (DutyKeywords.Any(dk => turnContext.Activity.Text.Contains(dk)))
            {
                var duty = string.Join(", ", new { turnContext.Activity.From.Name, turnContext.Activity.From.Id });
                var reply = BotMessageHelper.CreateMessageWithMention(turnContext.Activity.From, turnContext.Activity.From.Name, "Duty today: {0}");
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
        }
    }
}
