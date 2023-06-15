using DutyBot.Common.Contracts;
using DutyBot.Helpers;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

using System.Threading;

namespace DutyBot.BotActivityHandlers
{
    public class MessageActivityWithMetionHelper : IMessageActivityHelper
    {
        public async Task HandleMessageTurnContext(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Text.Contains("<at>"))
            {
                foreach (var entity in turnContext.Activity.Entities)
                {
                    if (entity != null && entity.Type == "mention")
                    {
                        var mension = entity.Properties.ToObject<Mention>();
                        var reply = BotMessageHelper.CreateMessageWithMention(mension.Mentioned, mension.Mentioned.Name, "hello {0}");
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                    }
                }
            }
        }
    }
}
