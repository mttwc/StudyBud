using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using StudyBud.Model;
using StudyBud.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyBud
{
    [Serializable]
    public class QuizDialog : IDialog<object>
    {
        private IList<Question> questions;
        private ScoreRecorder scoreRecorder;

        public async Task StartAsync(IDialogContext context)
        {
            Init(context);
            await context.PostAsync($"Starting the quiz for **{questions[0].Grade} {questions[0].Subject}, {questions[0].Topic}**"); // Being lazy
            await PostQuestionAsync(context);
            context.Wait(WaitForAnswerAsync);
        }

        private void Init(IDialogContext context)
        {
            scoreRecorder = new ScoreRecorder();

            // TODO: for demo purposes, we don't shuffle
            var grade = context.PerUserInConversationData.Get<string>(Keys.GRADE);
            var subject = context.PerUserInConversationData.Get<string>(Keys.SUBJECT);
            var topic = context.PerUserInConversationData.Get<string>(Keys.TOPIC);
            questions = QuestionBag.Instance.GetQuestions(grade, subject, topic);
        }

        private async Task PostQuestionAsync(IDialogContext context)
        {
            var curQuestion = questions[scoreRecorder.TotalCount];

            await context.PostAsync("**" + curQuestion.Body + "**");

            var choiceStr = Strings.QUIZ_MSG_QUESTIONPROMPT;
            var choices = curQuestion.Choices.Split(';');
            for (var i = 0; i < choices.Length; i++)
            {
                char answerAsChar = IntToLetterUpperCase(i);
                choiceStr += $"\n\nAnswer [**{answerAsChar}**]: {choices[i]}.";
            }

            await context.PostAsync(choiceStr);
        }

        public async Task WaitForAnswerAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (message.Text.Equals(Commands.END, StringComparison.OrdinalIgnoreCase))
            {
                await DoneAsync(context);
            }
            else
            {
                if (IsValidChoice(message.Text))
                {
                    var curQuestion = questions[scoreRecorder.TotalCount];

                    char choiceAsChar = char.ToUpper(message.Text[0]);
                    int choice = LetterToIntIgnoreCase(choiceAsChar);
                    bool isCorrectChoice = IsCorrectChoice(curQuestion, choice);

                    var response = $"You selected: {choiceAsChar}. ";
                    if (isCorrectChoice)
                    {
                        response += "**That is correct! " + PraiseBag.GetRandomPraise() + "**";
                    }
                    else
                    {
                        int actualAnswerAsInt = int.Parse(curQuestion.Answer);
                        char actualAsnwerAsChar = IntToLetterUpperCase(actualAnswerAsInt);
                        response += $"**The actual answer is: {actualAsnwerAsChar}** ({GetAnswer(curQuestion, actualAnswerAsInt)})";
                    }
                    await context.PostAsync(response);

                    scoreRecorder.Record(isCorrectChoice);

                    await PostFeedbackPromptAsync(context);
                }
                else
                {
                    // Loop
                    await context.PostAsync(Strings.QUIZ_MSG_SELECT_ANSWER_INSTRUCTION);
                    context.Wait(WaitForAnswerAsync);
                }
            }
        }

        public async Task PostFeedbackPromptAsync(IDialogContext context)
        {
            string feedbackPrompt = "**Did you like that question?**";
            feedbackPrompt += "\n\nChoice [**A**]: 👍";
            feedbackPrompt += "\n\nChoice [**B**]: 👎";
            feedbackPrompt += "\n\nChoice [**C**]: I prefer not to answer.";
            await context.PostAsync(feedbackPrompt);

            context.Wait(WaitForFeedbackAsync);
        }

        public async Task WaitForFeedbackAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (message.Text.Equals("a", StringComparison.OrdinalIgnoreCase))
            {
                await context.PostAsync(Strings.QUIZ_MSG_THANKS_FOR_FEEDBACK);
                await PrepareNextQuestionAsync(context);
            }
            else if (message.Text.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                await context.PostAsync(Strings.QUIZ_MSG_THANKS_FOR_FEEDBACK);
                await PrepareNextQuestionAsync(context);
            }
            else if (message.Text.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                await PrepareNextQuestionAsync(context);
            }
            else
            {
                // Loop
                await context.PostAsync(Strings.QUIZ_MESSAGE_INVALID_RESPONSE);
                context.Wait(WaitForFeedbackAsync);
            }
        }

        public async Task PrepareNextQuestionAsync(IDialogContext context)
        {
            if (scoreRecorder.TotalCount == questions.Count)
            {
                // No more questions
                await ShowScorecardAsync(context);
            }
            else
            {
                await PostQuestionAsync(context);
                context.Wait(WaitForAnswerAsync);
            }
        }

        public async Task ShowScorecardAsync(IDialogContext context)
        {
            var scorecard = $"***You answered {scoreRecorder.TotalCount} questions and got {scoreRecorder.CorrectCount} correct!***";
            await context.PostAsync(scorecard);

            // TODO: Check if user is auth'd
            var postToOneNoteStr = "**Would you like to post your score card to OneNote?**";
            postToOneNoteStr += "\n\nChoice [**A**]: Yes";
            postToOneNoteStr += "\n\nChoice [**B**]: No";
            await context.PostAsync(postToOneNoteStr);

            context.Wait(WaitingForPostToOneNoteAsync);
        }

        public async Task WaitingForPostToOneNoteAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            if (message.Text.Equals("a", StringComparison.OrdinalIgnoreCase))
            {
                // TODO
                await context.PostAsync("Done!");
                await DoneAsync(context);
            }
            else if (message.Text.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                await DoneAsync(context);
            }
            else
            {
                // Loop
                await context.PostAsync(Strings.QUIZ_MESSAGE_INVALID_RESPONSE);
                context.Wait(WaitingForPostToOneNoteAsync);
            }
        }

        public async Task DoneAsync(IDialogContext context)
        {
            context.Done(string.Empty);
        }

        private static bool IsValidChoice(string choice)
        {
            return choice.Length == 1 && char.IsLetter(choice[0]);
        }

        private static int LetterToIntIgnoreCase(char ch)
        {
            ch = char.ToUpper(ch);
            return ch - 65;
        }

        public static char IntToLetterUpperCase(int i)
        {
            return (char)(i + 65);
        }

        private static bool IsCorrectChoice(Question question, int choiceIndex)
        {
            return choiceIndex == int.Parse(question.Answer);
        }

        private static string GetAnswer(Question question, int index)
        {
            return question.Choices.Split(';')[index];
        }
    }
}