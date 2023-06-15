using Microsoft.Bot.Builder;

namespace DutyBot.Common.Contracts
{
    public interface IMessageActivityHelper
    {
        Task HandleMessageTurnContext(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}