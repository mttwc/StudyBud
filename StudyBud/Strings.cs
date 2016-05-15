namespace StudyBud
{
    public class Strings
    {
        // Menu
        public const string MENU_MSG_END = "**The quiz has ended!**\n\n" + SYSTEM_MSG_OPTIONS;

        // System
        public const string SYSTEM_MSG_ADDEDTOCONVO = "**Hi there! Please type one of the following options to interact with me!**\n\n" + SYSTEM_MSG_OPTIONS;

        public const string SYSTEM_MSG_OPTIONS = "[**Start**]: begins the demo quiz." +
            "\n\n[**Preferences**]: lets you set your user preferences.";

        // Quiz
        public const string QUIZ_MSG_END = "**The quiz has ended!**\n\n" + SYSTEM_MSG_OPTIONS;
        public const string QUIZ_MSG_QUESTIONPROMPT = "**Enter the letter of the answer you wish to select.**";

        // QuizPicker
        public const string QUIZPICKER_MSG_INITIAL = "Changing your quiz preferences!";

        public const string QUIZPICKER_MSG_SAVING = "Your selection is complete! Give me a second while I get your quiz ready ...";
        public const string QUIZPICKER_MSG_END = "**Your preferences have been saved!**\n\n" + SYSTEM_MSG_OPTIONS;
    }
}