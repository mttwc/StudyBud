using System;

namespace StudyBud.Model
{
    [Serializable]
    public class Question
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string Choices { get; set; }
        public string Answer { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Topic { get; set; }
    }
}