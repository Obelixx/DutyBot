using DutyBot.Common.Contracts;
using DutyBot.Helpers;
using DutyBot.Models;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Graph.Models;

using System.Text;

namespace DutyBot.BotActivityHandlers
{
    public class MessageActivityWithDuty : IMessageActivityHelper
    {
        public readonly string[] DutyKeywords = { "duty", "on air", "on watch" };
        public readonly string[] AllDutiesKeyword = { "duties" };
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
                await ResponseToKeyword(turnContext, cancellationToken);
            }

            if (AllDutiesKeyword.Any(dk => turnContext.Activity.Text.Contains(dk, StringComparison.InvariantCultureIgnoreCase)))
            {
                await ShowAllDuties(turnContext, cancellationToken);
            }

            if (turnContext.Activity.Text.Contains("|"))
            {
                dutyStorage.CleanUp(turnContext.Activity.Conversation.Id);
                await HandleMarkdownInput(turnContext, cancellationToken);
            }

            if (turnContext.Activity.Text.StartsWith($" {Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}"))
            {
                dutyStorage.CleanUp(turnContext.Activity.Conversation.Id);
                await HandleTableInput(turnContext, cancellationToken);
            }
        }

        private async Task ShowAllDuties(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var all = dutyStorage.GetAllDuties(turnContext.Activity.Conversation.Id);
            if (all.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Here are all saved duties:");
                sb.AppendLine();
                sb.AppendLine("| Date | Name |");
                sb.AppendLine("|------|------|");
                foreach (var duty in all)
                {
                    sb.AppendLine($"| {duty.Date.ToString("yyyy-MM-dd")} | {duty.Name} |");
                }

                await turnContext.SendActivityAsync(sb.ToString(), cancellationToken: cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync("Currently there are no saved duties.", cancellationToken: cancellationToken);
                await SendTutorial(turnContext, cancellationToken);
            }
        }

        private async Task HandleTableInput(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var tableFound = false;
            var savedCount = 0;
            var nl = Environment.NewLine;
            var tableStart = $" {nl}{nl}{nl}{nl}Date{nl}Name{nl}{nl}{nl}"; // our very specific table
            var tableEnd = $"{nl}{nl}{nl}{nl} ";
            try
            {
                var tableText = turnContext.Activity.Text.Split(tableStart, StringSplitOptions.None)[1];
                if (tableText == null)
                {
                    return;
                }

                tableFound = true;

                tableText = tableText.Split(tableEnd, StringSplitOptions.None)[0];
                if (tableText == null)
                {
                    return;
                }

                var cells = tableText.Split(nl, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                if (cells.Any() && cells.Count % 2 == 0)
                {
                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (DateTime.TryParse(cells[i], out var date))
                        {
                            var duty = new DutyModel(date, cells[++i]);
                            dutyStorage.SaveDuty(turnContext.Activity.Conversation.Id, duty);
                            savedCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            if (tableFound)
            {
                await turnContext.SendActivityAsync($"{savedCount} duties found and saved!", cancellationToken: cancellationToken);
            }
        }

        private async Task HandleMarkdownInput(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var tableFound = false;
            var savedCount = 0;
            var lines = turnContext.Activity.Text.Split(Environment.NewLine).Select(l => l.Trim()).ToList();
            for (int l = 0; l < lines.Count; l++)
            {
                var row = lines[l];
                if (row.StartsWith("|") && row.EndsWith("|"))
                {
                    var cells = row.Split("|", StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
                    if (tableFound)
                    {
                        if (cells[0].StartsWith("---"))
                        {
                            continue;
                        }
                        else
                        {
                            if (DateTime.TryParse(cells[0], out var date))
                            {
                                var duty = new DutyModel(date, cells[1]);
                                dutyStorage.SaveDuty(turnContext.Activity.Conversation.Id, duty);
                                savedCount++;
                            }
                        }
                    }

                    if (cells.Count == 2 &&
                        cells[0].Equals("Date", StringComparison.InvariantCultureIgnoreCase) &&
                        cells[1].Equals("Name", StringComparison.InvariantCultureIgnoreCase)) // start of our table
                    {
                        tableFound = true;
                    }
                }
                else if (tableFound)
                {
                    // table is found but we are outside of it
                    break;
                }
            }

            if (tableFound)
            {
                await turnContext.SendActivityAsync($"{savedCount} duties found and saved!", cancellationToken: cancellationToken);
            }
        }

        private async Task ResponseToKeyword(ITurnContext turnContext, CancellationToken cancellationToken = default)
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

        private async Task SendTutorial(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await turnContext.SendActivityAsync("To use the bot send a table containing duties in markdown format. Like that:", cancellationToken: cancellationToken);
            await turnContext.SendActivityAsync("| Date       | Name       |\r\n|------------|------------|\r\n| 2023-06-01 | Qdele Vance |\r\n| 2023-06-02 | Alex Wilber |", cancellationToken: cancellationToken);
        }
    }
}
