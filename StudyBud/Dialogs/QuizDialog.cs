using Microsoft.Bot.Builder.Dialogs;
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
        private int correctAnswers = 0;

        private string curSubject;
        private string curDifficulty;
        private IList<Question> questions;

        public async Task StartAsync(IDialogContext context)
        {
            await ThingAsync(context);
        }

        public async Task ThingAsync(IDialogContext context)
        {
            await context.PostAsync("Grade: " + context.PerUserInConversationData.Get<string>(Keys.GRADE));
            await context.PostAsync("Subject: " + context.PerUserInConversationData.Get<string>(Keys.SUBJECT));
            await context.PostAsync("Topic: " + context.PerUserInConversationData.Get<string>(Keys.TOPIC));
            context.Done(string.Empty);
        }

        //public async Task WaitingOnStartAsync(IDialogContext context, IAwaitable<Message> argument)
        //{
        //    var message = await argument;
        //    if (message.Text.ToLower() == "start")
        //    {
        //        var subjectsStr = "**Let the quiz begin! Please pick a subject:**";
        //        var subjects = questionBag.Subjects;
        //        foreach (var subject in subjects)
        //        {
        //            subjectsStr += $"\n\nChoose [**{subject}**]";
        //        }
        //        await context.PostAsync(subjectsStr);

        //        context.Wait(ChooseSubjectAsync);
        //    }
        //    else
        //    {
        //        await context.PostAsync("Type [**Start**] to begin the quiz!");
        //        context.Wait(WaitingOnStartAsync);
        //    }
        //}

        //public async Task ChooseSubjectAsync(IDialogContext context, IAwaitable<Message> argument)
        //{
        //    var message = await argument;
        //    var capitalized = message.Text.Substring(0, 1).ToUpper() + message.Text.Substring(1).ToLower();

        //    if (message.Text.ToLower() == "reset")
        //    {
        //        await AfterResetAsync(context);
        //    }
        //    else if (message.Text.ToLower() == "end")
        //    {
        //        await ScorecardAsync(context);
        //    }
        //    else if (questionBag.Subjects.Contains(capitalized))
        //    {
        //        curSubject = capitalized;

        //        var difficultiesStr = "**Please pick a difficulty:**";
        //        var difficulties = questionBag.GetDifficulties(curSubject);
        //        foreach (var difficulty in difficulties)
        //        {
        //            difficultiesStr += $"\n\nChoose [**{difficulty}**]";
        //        }
        //        await context.PostAsync(difficultiesStr);

        //        context.Wait(ChooseDifficultyAsync);
        //    }
        //    else
        //    {
        //        await context.PostAsync("Sorry, that subject does not exist. Please try again.");
        //        context.Wait(ChooseSubjectAsync);
        //    }
        //}

        //public async Task ChooseDifficultyAsync(IDialogContext context, IAwaitable<Message> argument)
        //{
        //    var message = await argument;
        //    var capitalized = message.Text.Substring(0, 1).ToUpper() + message.Text.Substring(1).ToLower();

        //    if (message.Text.ToLower() == "reset")
        //    {
        //        await AfterResetAsync(context);
        //    }
        //    else if (message.Text.ToLower() == "end")
        //    {
        //        await ScorecardAsync(context);
        //    }
        //    else if (questionBag.GetDifficulties(curSubject).Contains(capitalized))
        //    {
        //        curDifficulty = capitalized;
        //        questions = questionBag.GetQuestions(curSubject, curDifficulty);
        //        await PostQuestion(context, curQuestion);
        //        context.Wait(QuizAsync);
        //    }
        //    else
        //    {
        //        await context.PostAsync($"Sorry, that subject does not exist for subject {curSubject}. Please try again.");
        //        context.Wait(ChooseDifficultyAsync);
        //    }
        //}

        //public async Task QuizAsync(IDialogContext context, IAwaitable<Message> argument)
        //{
        //    var message = await argument;
        //    if (message.Text.ToLower() == "reset")
        //    {
        //        await AfterResetAsync(context);
        //    }
        //    else if (message.Text.ToLower() == "end")
        //    {
        //        await ScorecardAsync(context);
        //    }
        //    else
        //    {
        //        if (message.Text.Length == 1 && char.IsLetter(message.Text[0]))
        //        {
        //            char choiceAsChar = char.ToUpper(message.Text[0]);
        //            int choice = choiceAsChar - 65;

        //            var response = $"You selected: {choiceAsChar}. ";
        //            if (choice == int.Parse(this.questions[curQuestion].Answer))
        //            {
        //                response += "**That is correct! " + PraiseBag.GetRandomPraise() + "**";
        //                correctAnswers++;
        //            }
        //            else
        //            {
        //                int actualAnswerAsInt = int.Parse(this.questions[curQuestion].Answer);
        //                char actualAsnwerAsChar = (char)(actualAnswerAsInt + 65);
        //                response += $"**The actual answer is: {actualAsnwerAsChar}** ({this.questions[curQuestion].Choices.Split(';')[actualAnswerAsInt]})";
        //            }
        //            await context.PostAsync(response);

        //            this.curQuestion++;

        //            await GetFeedbackForQuestionAsync(context);
        //        }
        //        else
        //        {
        //            await context.PostAsync("Please type in the letter of the answer you wish to select.");
        //            context.Wait(QuizAsync);
        //        }
        //    }
        //}

        //private async Task PostQuestion(IDialogContext context, int index)
        //{
        //    await context.PostAsync("**" + this.questions[this.curQuestion].Body + "**");
        //    var choiceStr = "**Enter the letter of the answer you wish to select.**";
        //    var choices = this.questions[this.curQuestion].Choices.Split(';');
        //    for (var i = 0; i < choices.Length; i++)
        //    {
        //        char answerAsChar = (char)(i + 65);
        //        choiceStr += $"\n\nAnswer [**{answerAsChar}**]: {choices[i]}.";
        //    }
        //    await context.PostAsync(choiceStr);
        //}

        //public async Task GetFeedbackForQuestionAsync(IDialogContext context)
        //{
        //    string feedbackPrompt = "**Did you like that question?**";
        //    feedbackPrompt += "\n\nChoice [**A**]: 👍";
        //    feedbackPrompt += "\n\nChoice [**B**]: 👎";
        //    feedbackPrompt += "\n\nChoice [**C**]: I prefer not to answer.";
        //    await context.PostAsync(feedbackPrompt);
        //    context.Wait(WaitingOnFeedbackAsync);
        //}

        //public async Task WaitingOnFeedbackAsync(IDialogContext context, IAwaitable<Message> argument)
        //{
        //    var message = await argument;
        //    var input = message.Text.ToLower();
        //    if (input.Equals("a"))
        //    {
        //        await context.PostAsync("Thank you for your input!");
        //        await PrepareNextQuestionAsync(context);
        //    }
        //    else if (input.Equals("b"))
        //    {
        //        await context.PostAsync("Thank you for your input!");
        //        await PrepareNextQuestionAsync(context);
        //    }
        //    else if (input.Equals("c"))
        //    {
        //        await PrepareNextQuestionAsync(context);
        //    }
        //    else
        //    {
        //        await context.PostAsync("Sorry, that was not a valid response.");
        //        context.Wait(WaitingOnFeedbackAsync);
        //    }
        //}

        //public async Task PrepareNextQuestionAsync(IDialogContext context)
        //{
        //    if (curQuestion == questions.Count - 1)
        //    {
        //        await ScorecardAsync(context);
        //    }
        //    else
        //    {
        //        await PostQuestion(context, this.curQuestion);
        //        context.Wait(QuizAsync);
        //    }
        //}

        //public async Task ScorecardAsync(IDialogContext context)
        //{
        //    var scorecard = $"***You answered {curQuestion} questions and got {correctAnswers} correct!***";
        //    await context.PostAsync(scorecard);

        //    var postToOneNoteStr = "**Would you like to post your score card to OneNote?**";
        //    postToOneNoteStr += "\n\nChoice [**A**]: Yes";
        //    postToOneNoteStr += "\n\nChoice [**B**]: No";
        //    await context.PostAsync(postToOneNoteStr);

        //    context.Wait(WaitingForPostToOneNoteAsync);
        //}

        //public async Task WaitingForPostToOneNoteAsync(IDialogContext context, IAwaitable<Message> argument)
        //{
        //    var message = await argument;
        //    var input = message.Text.ToLower();
        //    if (input.Equals("a"))
        //    {
        //        await context.PostAsync("Done!");
        //        await AfterResetAsync(context);
        //    }
        //    else if (input.Equals("b"))
        //    {
        //        await AfterResetAsync(context);
        //    }
        //    else
        //    {
        //        await context.PostAsync("Sorry, that was not a valid response.");
        //        context.Wait(WaitingForPostToOneNoteAsync);
        //    }
        //}

        //public async Task AfterResetAsync(IDialogContext context)
        //{
        //    ResetState();
        //    await context.PostAsync("Demo reset. Type [**Start**] to begin the quiz!");
        //    context.Wait(WaitingOnStartAsync);
        //}

        //private void ResetState()
        //{
        //    curQuestion = 0;
        //    correctAnswers = 0;
        //    curSubject = null;
        //    curDifficulty = null;
        //    questions = null;
        //}
    }
}