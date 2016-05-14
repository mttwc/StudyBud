using StudyBud.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyBud.Model
{
    [Serializable]
    public class QuestionBag
    {
        private IDictionary<string, IList<string>> difficultiesPerSubject;
        private IDictionary<string, IList<Question>> questionsPerSubject;

        public ICollection<string> Subjects
        {
            get;
            private set;
        }

        public QuestionBag(string path)
        {
            var questions = Parser.Parse<Question>(path);

            difficultiesPerSubject = new Dictionary<string, IList<string>>();
            questionsPerSubject = new Dictionary<string, IList<Question>>();

            foreach (var question in questions)
            {
                if (!difficultiesPerSubject.Keys.Contains(question.Subject))
                {
                    difficultiesPerSubject.Add(question.Subject, new List<string>());
                    questionsPerSubject.Add(question.Subject, new List<Question>());
                }

                if (!difficultiesPerSubject[question.Subject].Contains(question.Rating))
                {
                    difficultiesPerSubject[question.Subject].Add(question.Rating);
                }
                questionsPerSubject[question.Subject].Add(question);
            }
            Subjects = difficultiesPerSubject.Keys;
        }

        public IList<string> GetDifficulties(string subject)
        {
            return difficultiesPerSubject[subject];
        }

        public IList<Question> GetQuestions(string subject)
        {
            return questionsPerSubject[subject];
        }

        public IList<Question> GetQuestions(string subject, string difficulty)
        {
            return GetQuestions(subject).Where(q => q.Rating.Equals(difficulty)).ToList();
        }
    }
}