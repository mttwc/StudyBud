using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using StudyBud.Model;
using StudyBud.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyBud
{
    [Serializable]
    public class QuizDialog : IDialog<object>
    {
        private List<Question> questions;
        private int curQuestion = 0;

        public async Task StartAsync(IDialogContext context)
        {
            questions = Parser.Parse<Question>(@"Y:\Programming\Projects\Bot Framework\StudyBud\StudyBud\Persistence\StudyBud.csv");
            context.Wait(WaitingOnStartAsync);
        }

        public async Task WaitingOnStartAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            if (message.Text == "start")
            {
                await context.PostAsync("Let the quiz begin!");
                await PostQuestion(context, curQuestion);

                context.Wait(QuizAsync);
            }
            else
            {
                await context.PostAsync("Type 'start' to begin the quiz!");
                context.Wait(WaitingOnStartAsync);
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
                    var response = $"You selected: {choice}. ";
                    response += choice == int.Parse(this.questions[curQuestion].Answer) ?
                        "That is correct!" :
                        response += $"The actual answer is: {this.questions[curQuestion].Answer}";
                    await context.PostAsync(response);

                    if (curQuestion == questions.Count - 1)
                    {
                        curQuestion = 0;
                        await context.PostAsync("You have completed the demo! Type 'start' to begin the quiz again!");
                        context.Wait(WaitingOnStartAsync);
                    }
                    else
                    {
                        await PostQuestion(context, ++this.curQuestion);
                        context.Wait(QuizAsync);
                    }
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
                curQuestion = 0;
                await context.PostAsync("Demo reset.");
                context.Wait(WaitingOnStartAsync);
            }
            else
            {
                await context.PostAsync("Demo was not reset.");
                context.Wait(QuizAsync);
            }
        }

        private async Task PostQuestion(IDialogContext context, int index)
        {
            await context.PostAsync(this.questions[this.curQuestion].Body);
            var choiceStr = "Enter the number of the answer you wish to select.";
            var choices = this.questions[this.curQuestion].Choices.Split(';');
            for (var i = 0; i < choices.Length; i++)
            {
                choiceStr += $"\n\nAnswer [{i}]: {choices[i]}.";
            }
            await context.PostAsync(choiceStr);
        }
    }
}