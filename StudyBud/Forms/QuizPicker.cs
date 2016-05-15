using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using StudyBud.Model;
using System;
using System.Threading.Tasks;

namespace StudyBud.Forms
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Sorry, I didn't understand \"**{0}**\". **Please try again.**")]
    [Template(TemplateUsage.EnumSelectOne, "**Please select a {&}.** {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
    public class QuizPicker
    {
        private static QuestionBag questionBag = QuestionBag.Instance;

        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }

        public static IForm<QuizPicker> BuildForm()
        {
            return new FormBuilder<QuizPicker>()
                .Message(Strings.QUIZPICKER_MSG_INITIAL)
                .Field(new FieldReflector<QuizPicker>(nameof(Grade))
                    .SetType(null)
                    .SetDefine(DefineGrades))
                .Field(new FieldReflector<QuizPicker>(nameof(Subject))
                    .SetType(null)
                    .SetDefine(DefineSubjects))
                .Field(new FieldReflector<QuizPicker>(nameof(Topic))
                    .SetType(null)
                    .SetDefine(DefineTopics))
                .OnCompletionAsync(CompletionCallback)
                .Build();
        }

        private static Task<bool> DefineGrades(QuizPicker state, Field<QuizPicker> field)
        {
            foreach (var educationLevel in questionBag.GetGrades())
            {
                field
                    .AddDescription(educationLevel, educationLevel)
                    .AddTerms(educationLevel, educationLevel);
            }
            return Task.FromResult(true);
        }

        private static Task<bool> DefineSubjects(QuizPicker state, Field<QuizPicker> field)
        {
            foreach (var subject in questionBag.GetSubjects(state.Grade))
            {
                field
                    .AddDescription(subject, subject)
                    .AddTerms(subject, subject);
            }
            return Task.FromResult(true);
        }

        private static Task<bool> DefineTopics(QuizPicker state, Field<QuizPicker> field)
        {
            foreach (var topic in questionBag.GetTopics(state.Grade, state.Subject))
            {
                field
                    .AddDescription(topic, topic)
                    .AddTerms(topic, topic);
            }
            return Task.FromResult(true);
        }

        private static Task<QuizPicker> CompletionCallback(IDialogContext context, QuizPicker state)
        {
            //context.Done(state);
            return Task.FromResult(state);
        }
    }
}