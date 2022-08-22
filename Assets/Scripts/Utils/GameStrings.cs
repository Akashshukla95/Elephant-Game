using System;

namespace ElesJourney.GlobalStrings
{
    [Serializable]
    public static class GameStrings
    {
        #region ------------------ Class Strings ------------------------------
        public const string menuScene = "Menu";
        public const string narrationScene = "Narration";
        public const string gameScene = "Game";
        public const string jsonDialogueFile = "Dialogues";
        public const string userData = "UserData";
        public const string unlockedItemsPrefs = "UnlockedItemsPrefs";
        public const string itemUnlocked = "You have unlocked ";

        #endregion ----------------------------------------------------------------


        #region ------------------- Gameplay/Dialogues Strings --------------------
        public static string[] wordCompletedStrings =
        {
            "Hey! I can still see some letters on the letter bar! Clear it please ! ",
            "I am so excited! Let's go ahead. Clear everything!!",
            "Please clear the slots, let’s continue!"
        };

        #endregion ----------------------------------------------------------------


        #region ----------------------- Animation Strings --------------------------

        public const string defaultStateAnim = "Default";
        public const string scaleInAnimation = "ScaleIn";
        public const string scaleOutAnimation = "ScaleOut";
        public const string wrongAnswer = "WrongAnswer";
        public const string wordIntroAnimation = "WordIntro";
        public const string wordOutroAnimation = "Outro";
        public const string wordClosureAnimation = "WordSpriteScaleOut";
        public const string spinWheelIntro = "SpinWheelIntro";
        public const string spinWheelExit = "SpinWheelExit";
        public const string collectAnimation = "CollectAnimation";
        public const string itemUnlockedAnimation = "UnlockItem";

        #endregion ----------------------------------------------------------------

        #region  --------------------- Sound Names --------------------------------

        #endregion ----------------------------------------------------------------
    }
}

