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
        private static IDialog<object> MakeQuizPickerDialog()
        {
            return Chain
                .PostToChain()
                .Switch(
                    new Case<Message, IDialog<string>>((msg) =>
                    {
                        var regex = new Regex("^preferences", RegexOptions.IgnoreCase);
                        return regex.IsMatch(msg.Text);
                    }, (context, message) =>
                    {
                        //context.PerUserInConversationData.SetValue("grade", quizPickerResult.Grade);
                        //context.PerUserInConversationData.SetValue("subject", quizPickerResult.Subject);
                        //context.PerUserInConversationData.SetValue("topic", quizPickerResult.Topic);
                        //return Chain.ContinueWith(QuizPicker.BuildForm());
                        return Chain.ContinueWith(new QuizDialog(),
                                async (ctx, res) =>
                                {
                                    var msaInfo = await res;
                                    return Chain.Return("preferences finish");
                                });
                    }),
                    new Case<Message, IDialog<string>>((msg) =>
                    {
                        var regex = new Regex("^start", RegexOptions.IgnoreCase);
                        return regex.IsMatch(msg.Text);
                    }, (context, message) =>
                    {
                        var quizPicker = Chain.From(() => FormDialog.FromForm(QuizPicker.BuildForm));
                        return Chain.ContinueWith(quizPicker,
                                async (ctx, res) =>
                                {
                                    var msaInfo = await res;
                                    await context.PostAsync("start finish");
                                    return Chain.Return("start finish");
                                });
                    }))
                .Unwrap()
                .PostToUser();
                //.From(() => FormDialog.FromForm(QuizPicker.BuildForm))
                //.Do(async (context, selection) =>
                //{
                //    try
                //    {
                //        var quizPickerResult = await selection;
                //        context.PerUserInConversationData.SetValue("grade", quizPickerResult.Grade);
                //        context.PerUserInConversationData.SetValue("subject", quizPickerResult.Subject);
                //        context.PerUserInConversationData.SetValue("topic", quizPickerResult.Topic);
                //    }
                //    catch (FormCanceledException<QuizPicker> fce)
                //    {
                //        await context.PostAsync("Quiz canceled");
                //    }
                //    // Need FormOptions.PromptInStart to start next dialog 
                //});
        }

        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                //return await Conversation.SendAsync(message, () => new QuizDialog());
                return await Conversation.SendAsync(message, MakeQuizPickerDialog);
                //if (message.Text.Equals("start", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    return await Conversation.SendAsync(message, () => new QuizDialog());
                //}
                //else if (message.Text.Equals("preferences", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    return await Conversation.SendAsync(message, MakeQuizPickerDialog);
                //}
                //else
                //{
                //    return null;
                //}
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