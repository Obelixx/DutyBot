using DutyBot.Common.Contracts;
using DutyBot.Helpers;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Graph.Models.TermStore;

namespace DutyBot.BotActivityHandlers
{
    public class MessageActivityWithDuty : IMessageActivityHelper
    {
        public readonly string[] DutyKeywords = { "duty", "on air", "on watch" };
        private readonly string DutyMessageFormat = "On duty today: {0}";
        private readonly IDutyStorage dutyStorage;
        private readonly IMSGraphHelper msGraph;

        public MessageActivityWithDuty(IDutyStorage dutyStorage, IMSGraphHelper msGraph)
        {
            this.dutyStorage = dutyStorage;
            this.msGraph = msGraph;
        }

        public async Task HandleMessageTurnContext(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (DutyKeywords.Any(dk => turnContext.Activity.Text.Contains(dk, StringComparison.InvariantCultureIgnoreCase)))
            {
                // check today duty - if not found replay with tutorial
                var dutyToday = dutyStorage.DutyToday(turnContext.Activity.Conversation.Id);
                if (dutyToday == null)
                {
                    await turnContext.SendActivityAsync("Duty keyword detected, but today duty is unknown to me!", cancellationToken: cancellationToken);
                    await SendTutorial(turnContext, cancellationToken);
                }
                else
                {
                    try
                    {
                        // search for the user in the directory
                        var user = await msGraph.GetUserByNameAsync(dutyToday.Name);
                        if (user != null)
                        {
                            // if found -> mention
                            var ca = new ChannelAccount(user.Id, user.DisplayName, "user", user.SecurityIdentifier);
                            var reply = BotMessageHelper.CreateMessageWithMention(ca, user.DisplayName, DutyMessageFormat);
                            await turnContext.SendActivityAsync(reply, cancellationToken);
                        }
                        else
                        {
                            // if not found just replay wiht the name
                            await turnContext.SendActivityAsync(string.Format(DutyMessageFormat, dutyToday.Name), cancellationToken: cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        await turnContext.SendActivityAsync(string.Format(DutyMessageFormat, dutyToday.Name), cancellationToken: cancellationToken);
                    }
                }
            }
        }

        private async Task SendTutorial(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await turnContext.SendActivityAsync("To use the bot send a table containing duties in markdown format. Like that:", cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync("| Date       | Name       |\r\n|------------|------------|\r\n| 2023-06-01 | Qdele Vance |\r\n| 2023-06-02 | Alex Wilber |", cancellationToken: cancellationToken);
        }
    }
}
