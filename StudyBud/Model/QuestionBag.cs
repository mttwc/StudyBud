using StudyBud.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyBud.Model
{
    /// <summary>
    /// A placeholder fake DAO that points to a CSV file on Matthew's laptop.
    /// </summary>
    [Serializable]
    public class QuestionBag
    {
        private static QuestionBag instance;

        // Not even a relative path, what a joke!
        private static string path = @"Y:\Programming\Projects\Bot Framework\StudyBud\StudyBud\Persistence\StudyBud.csv";

        public static QuestionBag Instance {
            get
            {
                if (instance == null)
                {
                    instance = new QuestionBag(path);
                }
                return instance;
            }
        }

        private IList<Question> questions;

        private QuestionBag(string path)
        {
            questions = Parser.Parse<Question>(path);
        }

        public IList<string> GetGrades()
        {
            return questions.Select(q => q.Grade).Distinct().OrderBy(g => g).ToList();
        }

        public IList<string> GetSubjects(string grade)
        {
            return questions.Where(q => q.Grade.Equals(grade)).Select(q => q.Subject).Distinct().OrderBy(s => s).ToList();
        }

        public IList<string> GetTopics(string grade, string subject)
        {
            return questions.Where(q => q.Grade.Equals(grade) && q.Subject.Equals(subject)).Select(q => q.Topic).Distinct().OrderBy(t => t).ToList();
        }

        public IList<Question> GetQuestions(string grade, string subject, string topic)
        {
            return questions.Where(q => q.Grade.Equals(grade) && q.Subject.Equals(subject) && q.Topic.Equals(topic)).OrderBy(q => q.Id).ToList();
        }
    }
}