namespace StudyBud.Model
{
    public class Question
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string[] Choices { get; set; }
        public int Answer { get; set; }
        public string Rating { get; set; }
        public string Subject { get; set; }
    }
}