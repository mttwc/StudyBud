using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using StudyBud.Forms;
using StudyBud.Model;
using System;
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
                                    return Chain.Return("start finish");
                                });
                    }),
                    new Case<Message, IDialog<string>>((msg) =>
                    {
                        var regex = new Regex("^preferences", RegexOptions.IgnoreCase);
                        return regex.IsMatch(msg.Text);
                    }, (context, message) =>
                    {
                        return Chain.ContinueWith(FormDialog.FromForm(QuizPicker.BuildForm),
                                async (ctx, msg) =>
                                {
                                    var result = await msg;
                                    return Chain.Return("preferences finish");
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
                var replyStr = "**Hi there! Please type one of the following options to interact with me!**";
                replyStr += "\n\n[**Start**]: begins the demo quiz.";
                return message.CreateReplyMessage(replyStr);
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