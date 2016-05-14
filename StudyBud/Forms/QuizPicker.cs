using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Threading.Tasks;

namespace StudyBud.Forms
{
    [Serializable]
    public class QuizPicker
    {
        public string EducationLevel { get; set; }

        public static IForm<QuizPicker> BuildForm()
        {
            return new FormBuilder<QuizPicker>()
                .Message("Hi there!")
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
                .OnCompletionAsync(async (context, quizPicker) =>
                {
                    await context.PostAsync("Selection done");
                })
                .Build();
        }

        static IList<string> GetEducationLevels()
        {
            return new List<string>
            {
                "Grade 1",
                "Grade 2",
                "Grade 3"
            };
        }
    }
}