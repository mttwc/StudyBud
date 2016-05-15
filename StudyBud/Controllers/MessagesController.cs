using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using StudyBud.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace StudyBud
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static IDialog<object> MakeDialog()
        {
            return Chain
                .PostToChain()
                .Switch(
                    new Case<Message, IDialog<string>>((msg) =>
                    {
                        var regex = new Regex("^start", RegexOptions.IgnoreCase);
                        return regex.IsMatch(msg.Text);
                    }, (context, message) =>
                    {
                        return Chain.ContinueWith(new QuizDialog(),
                                async (ctx, msg) =>
                                {
                                    var result = await msg;
                                    return Chain.Return(Strings.QUIZ_MSG_END);
                                });
                    }),
                    new Case<Message, IDialog<string>>((msg) =>
                    {
                        var regex = new Regex("^preferences", RegexOptions.IgnoreCase);
                        return regex.IsMatch(msg.Text);
                    }, (context, message) =>
                    {
                        return Chain.ContinueWith(FormDialog.FromForm(QuizPicker.BuildForm, FormOptions.PromptInStart),
                                async (ctx, msg) =>
                                {
                                    var result = await msg;
                                    await ctx.PostAsync(Strings.QUIZPICKER_MSG_SAVING);
                                    ctx.PerUserInConversationData.SetValue(Keys.GRADE, result.Grade);
                                    ctx.PerUserInConversationData.SetValue(Keys.SUBJECT, result.Subject);
                                    ctx.PerUserInConversationData.SetValue(Keys.TOPIC, result.Topic);
                                    return Chain.Return(Strings.QUIZPICKER_MSG_END);
                                });
                    }))
                .Unwrap()
                .PostToUser();
        }

        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                return await Conversation.SendAsync(message, MakeDialog);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
            }
            else if (message.Type == "BotAddedToConversation")
            {
                return message.CreateReplyMessage(Strings.SYSTEM_MSG_ADDEDTOCONVO);
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}