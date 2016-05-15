using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Threading.Tasks;
using StudyBud.Model;

namespace StudyBud.Forms
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Sorry, I didn't understand \"{0}\". Please try again.")]
    [Template(TemplateUsage.EnumSelectOne, "Please select a {&}. {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
    public class QuizPicker
    {
        private static QuestionBag questionBag = QuestionBag.Instance;

        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }

        public static IForm<QuizPicker> BuildForm()
        {
            return new FormBuilder<QuizPicker>()
                .Message("Starting a quiz!")
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

        static Task<bool> DefineGrades(QuizPicker state, Field<QuizPicker> field)
        {
            foreach (var educationLevel in questionBag.GetGrades())
            {
                field
                    .AddDescription(educationLevel, educationLevel)
                    .AddTerms(educationLevel, educationLevel);
            }
            return Task.FromResult(true);
        }

        static Task<bool> DefineSubjects(QuizPicker state, Field<QuizPicker> field)
        {
            foreach (var subject in questionBag.GetSubjects(state.Grade))
            {
                field
                    .AddDescription(subject, subject)
                    .AddTerms(subject, subject);
            }
            return Task.FromResult(true);
        }

        static Task<bool> DefineTopics(QuizPicker state, Field<QuizPicker> field)
        {
            foreach (var topic in questionBag.GetTopics(state.Grade, state.Subject))
            {
                field
                    .AddDescription(topic, topic)
                    .AddTerms(topic, topic);
            }
            return Task.FromResult(true);
        }

        static Task<QuizPicker> CompletionCallback(IDialogContext context, QuizPicker state)
        {
            context.PostAsync("Your selection is complete! Give me a second while I get your quiz ready ...");
            context.PerUserInConversationData.SetValue("grade", state.Grade);
            context.PerUserInConversationData.SetValue("subject", state.Subject);
            context.PerUserInConversationData.SetValue("topic", state.Topic);
            context.Done(state);
            return Task.FromResult(state);
        }
    }
}