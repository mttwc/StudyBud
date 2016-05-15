using System;

namespace StudyBud
{
    [Serializable]
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
        public const string QUIZ_MSG_SELECT_ANSWER_INSTRUCTION = "Please type in the letter of the answer you wish to select.";
        public const string QUIZ_MSG_THANKS_FOR_FEEDBACK = "Thanks for your feedback!";
        public const string QUIZ_MESSAGE_INVALID_RESPONSE = "Sorry, that was not a valid response.";

        // QuizPicker
        public const string QUIZPICKER_MSG_INITIAL = "Changing your quiz preferences!";

        public const string QUIZPICKER_MSG_END = "**Your preferences have been saved!**\n\n" + SYSTEM_MSG_OPTIONS;
    }
}