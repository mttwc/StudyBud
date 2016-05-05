using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using StudyBud.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace StudyBud
{
    [Serializable]
    public class QuizDialog : IDialog<object>
    {
        private List<Question> questions;
        private int curQuestion = 0;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            if (message.Text == "start")
            {
                await context.PostAsync("Let the quiz begin!");
                await context.PostAsync($"Question {this.curQuestion}");
                context.Wait(QuizAsync);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task QuizAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the demo?",
                    "Didn't get that!");
            }
            else
            {
                int choice;
                if (int.TryParse(message.Text, out choice))
                {
                    await context.PostAsync($"Question: {this.curQuestion} - Answer selected: {choice} - Actual answer: BLAH");
                    this.curQuestion++;
                    await context.PostAsync($"Question {this.curQuestion}");
                    context.Wait(QuizAsync);
                }
                else
                {
                    await context.PostAsync("Please type in the number of the answer you wish to select.");
                    context.Wait(QuizAsync);
                }
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.curQuestion = 0;
                await context.PostAsync("Reset demo.");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("Did not reset demo.");
                context.Wait(QuizAsync);
            }
        }
    }

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                return await Conversation.SendAsync(message, () => new QuizDialog());
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
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
                return message.CreateReplyMessage("Type 'start' to begin the quiz!");
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