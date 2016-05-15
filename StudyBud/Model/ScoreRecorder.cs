using System;

namespace StudyBud.Model
{
    [Serializable]
    public class ScoreRecorder
    {
        public int CorrectCount { get; private set; }
        public int TotalCount { get; private set; }

        public ScoreRecorder()
        {
            CorrectCount = 0;
            TotalCount = 0;
        }

        public void Record(bool isCorrect)
        {
            if (isCorrect)
            {
                CorrectCount++;
            }
            TotalCount++;
        }
    }
}