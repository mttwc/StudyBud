using System;

namespace StudyBud.Utils
{
    [Serializable]
    public class PraiseBag
    {
        private static string[] praises =
        {
            "Nice work!",
            "You're doing a great job!",
            "You're killing it!",
            "You're doing awesome work!",
            "All that hard work is paying off!",
            "Excellent work!",
            "Too easy!"
        };

        public static string GetRandomPraise()
        {
            Random r = new Random();
            int val = r.Next(0, praises.Length);
            return praises[val] + " ";
        }
    }
}