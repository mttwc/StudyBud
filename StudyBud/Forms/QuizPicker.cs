using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Threading.Tasks;

namespace StudyBud.Forms
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Sorry, I didn't understand \"{0}\". Please try again.")]
    [Template(TemplateUsage.EnumSelectOne, "Please select a {&}. {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
    public class QuizPicker
    {
        public string EducationLevel { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }

        public static IForm<QuizPicker> BuildForm()
        {
            return new FormBuilder<QuizPicker>()
                .Message("Starting a quiz!")
                .Field(new FieldReflector<QuizPicker>(nameof(EducationLevel))
                    .SetType(null)
                    .SetDefine((state, field) =>
                    {
                        foreach (var educationLevel in GetEducationLevels())
                            field
                                .AddDescription(educationLevel, educationLevel)
                                .AddTerms(educationLevel, educationLevel);
                        return Task.FromResult(true);
                    }))
                .Field(new FieldReflector<QuizPicker>(nameof(Subject))
                    .SetType(null)
                    .SetDefine((state, field) =>
                    {
                        foreach (var subject in GetSubjects(state.EducationLevel))
                        {
                            field
                                .AddDescription(subject, subject)
                                .AddTerms(subject, subject);
                        }
                        return Task.FromResult(true);
                    }))
                .Field(new FieldReflector<QuizPicker>(nameof(Topic))
                    .SetType(null)
                    .SetDefine((state, field) =>
                    {
                        foreach (var topic in GetTopics(state.EducationLevel, state.Subject))
                        {
                            field
                                .AddDescription(topic, topic)
                                .AddTerms(topic, topic);
                        }
                        return Task.FromResult(true);
                    }))
                .OnCompletionAsync(async (context, quizPicker) =>
                {
                    await context.PostAsync("Your selection is complete! Give me a second while I get your quiz ready ...");
                })
                .Build();
        }

        static IList<string> GetEducationLevels()
        {
            return new List<string>
            {
                "1st Grade",
                "2nd Grade",
                "3rd Grade"
            };
        }

        static IList<string> GetSubjects(string educationLevel)
        {
            return new List<string>
            {
                "Math",
                "Science"
            };
        }

        static IList<string> GetTopics(string educationLevel, string subject)
        {
            return new List<string>
            {
                "Placeholder"
            };
        }
    }
}