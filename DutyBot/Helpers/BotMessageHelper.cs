using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Xml;

namespace DutyBot.Helpers
{
    public static class BotMessageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account">turnContext.Activity.From</param>
        /// <param name="nameText">turnContext.Activity.From.Name</param>
        /// <param name="messageWithPlaceholder">Hello {0}.</param>
        /// <returns></returns>
        public static Activity CreateMessageWithMention(ChannelAccount accountToMention, string nameText, string messageWithPlaceholder)
        {
            var mention = new Mention
            {
                Mentioned = accountToMention,
                Text = $"<at>{XmlConvert.EncodeName(nameText)}</at>",
                Type = "mention",
            };

            var replyActivity = MessageFactory.Text(string.Format(messageWithPlaceholder, mention.Text));
            if (replyActivity.Entities == null)
            {
                replyActivity.Entities = new List<Microsoft.Bot.Schema.Entity>();
            }

            replyActivity.Entities.Add(mention);
            return replyActivity;
        }
    }
}
