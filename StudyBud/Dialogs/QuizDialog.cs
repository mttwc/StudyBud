using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using StudyBud.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyBud
{
    [Serializable]
    public class QuizDialog : IDialog<object>
    {
        private QuestionBag questionBag;
        private int curQuestion = 0;

        private string curSubject;
        private string curDifficulty;
        private List<Question> questions;

        public async Task StartAsync(IDialogContext context)
        {
            questionBag = new QuestionBag(@"Y:\Programming\Projects\Bot Framework\StudyBud\StudyBud\Persistence\StudyBud.csv");
            context.Wait(WaitingOnStartAsync);
        }

        public async Task WaitingOnStartAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            if (message.Text == "Start")
            {
                var subjectsStr = "Let the quiz begin! Please pick a subject:";
                var subjects = questionBag.Subjects;
                foreach (var subject in subjects)
                {
                    subjectsStr += $"\n\nChoose [{subject}]";
                }
                await context.PostAsync(subjectsStr);

                context.Wait(ChooseSubjectAsync);
            }
            else
            {
                await context.PostAsync("Type [Start] to begin the quiz!");
                context.Wait(WaitingOnStartAsync);
            }
        }

        public async Task ChooseSubjectAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (questionBag.Subjects.Contains(message.Text))
            {
                curSubject = message.Text;

                var difficultiesStr = "Please pick a difficulty:";
                var difficulties = questionBag.GetDifficulties(message.Text);
                foreach (var difficulty in difficulties)
                {
                    difficultiesStr += $"\n\nChoose [{difficulty}]";
                }
                await context.PostAsync(difficultiesStr);

                context.Wait(ChooseDifficultyAsync);
            }
            else
            {
                await context.PostAsync("Sorry, that subject does not exist. Please try again.");
                context.Wait(ChooseSubjectAsync);
            }
        }

        public async Task ChooseDifficultyAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (questionBag.GetDifficulties(curSubject).Contains(message.Text))
            {
                curDifficulty = message.Text;
                questions = questionBag.GetQuestions(curSubject, curDifficulty);
                await PostQuestion(context, curQuestion);
                context.Wait(QuizAsync);
            }
            else
            {
                await context.PostAsync($"Sorry, that subject does not exist for subject {curSubject}. Please try again.");
                context.Wait(ChooseDifficultyAsync);
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
                        await context.PostAsync("You have completed the demo! Type [Start] to begin the quiz again!");
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

        private void ResetState()
        {
            curQuestion = 0;
            curSubject = null;
            curDifficulty = null;
        }
    }
}