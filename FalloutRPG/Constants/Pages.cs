namespace FalloutRPG.Constants
{
    public class Pages
    {
        #region HELP GROUP
        #region Index Help Pages
        public static string[] HELP_PAGE1_TITLES = new string[]
        {
            "$help general",
            "$help character",
            "$help roll",
            "$help skills",
            "$help craps"
        };

        public static string[] HELP_PAGE1_CONTENTS = new string[]
        {
            "Displays general help menu.",
            "Displays character help menu.",
            "Displays roll help menu.",
            "Displays a list of skills.",
            "Displays craps help page."
        };
        #endregion

        #region General Help Pages
        public static string[] HELP_GENERAL_PAGE1_TITLES = new string[]
        {
            "$pay [@user] [amount]",
            "$daysleft",
            "$highscores"
        };

        public static string[] HELP_GENERAL_PAGE1_CONTENTS = new string[]
        {
            "Pays a user the specified amount of caps.",
            "Shows how many days are left until the release of Fallout 76 using UTC.",
            "Shows the top 10 characters ordered by experience."
        };
        #endregion

        #region Character Help Pages
        public static string[] HELP_CHAR_PAGE1_TITLES = new string[]
        {
            "$char show",
            "$char show [@user]",
            "$char create [name]",
            "$char story",
            "$char story [@user]",
            "$char story update [story]",
            "$char desc",
            "$char desc [@user]",
            "$char desc update [desc]"
        };

        public static string[] HELP_CHAR_PAGE1_CONTENTS = new string[]
        {
            "Displays your character.",
            "Displays specified user's character.",
            "Creates your character.",
            "Displays your character's story.",
            "Displays specified user's character story.",
            "Updates your character's story.",
            "Displays your character's description.",
            "Displays specified user's character description.",
            "Updates your character's description."
        };

        public static string[] HELP_CHAR_PAGE2_TITLES = new string[]
        {
            "$char stats",
            "$char stats [@user]",
            "$char highscores",
            "$char skills",
            "$char skills [@user]",
            "$char skills set [tag1] [tag2] [tag3]",
            "$char skills spend [skill] [points]",
            "$char special",
            "$char special [@user]",
            "$char special set [S] [P] [E] [C] [I] [A] [L]"
        };

        public static string[] HELP_CHAR_PAGE2_CONTENTS = new string[]
        {
            "Displays your level and experience.",
            "Displays specified user's level and experience.",
            "Displays the top 10 players ordered by experience.",
            "Displays your character's skills.",
            "Displays specified user's character skills.",
            "Sets your initial tag skills.",
            "Puts points in one of your skills.",
            "Displays your character's SPECIAL.",
            "Displays specified user's character SPECIAL.",
            "Sets your SPECIAL."
        };
        #endregion

        #region Roll Help Pages
        public static string[] HELP_ROLL_PAGE1_TITLES = new string[]
        {
            "$roll [skill]",
            "$roll [special]"
        };

        public static string[] HELP_ROLL_PAGE1_CONTENTS = new string[]
        {
            "Gets a roll result based on the skill level.",
            "Gets a roll result based on the SPECIAL level."
        };
        #endregion

        #region Craps Help Pages
        public static string[] HELP_CRAPS_PAGE1_TITLES = new string[]
        {
            "$craps join",
            "$craps leave",
            "$craps roll",
            "$craps bet [type] [amount]",
            "$craps pass",
            "Bet Types"
        };

        public static string[] HELP_CRAPS_PAGE1_CONTENTS = new string[]
        {
            "Joins current craps game.",
            "Leaves current craps game.",
            "Rolls the dice.",
            "Makes a bet of the type and amount. Types can be found below.",
            "Pass the dice to another user.",
            "pass, dontpass, come, dontcome"
        };
        #endregion

        #region Admin Help Pages
        public static string[] HELP_ADMIN_PAGE1_TITLES = new string[]
        {
            "$admin givemoney [@user] [amount]",
            "$admin giveskillpoints [@user] [amount]",
            "$admin changename [@user] [name]",
            "$admin reset [@user]"
        };

        public static string[] HELP_ADMIN_PAGE1_CONTENTS = new string[]
        {
            "Gives a character the specified amount of caps.",
            "Gives a character the specified amount of skill points.",
            "Changes a character's name.",
            "Resets a character's skill points and SPECIAL. They will then be able to use *$char skills claim* to claim their skill points back."
        };
        #endregion

        #region Tutorial
        public static string[] TUTORIAL_TITLES = new string[]
        {
            "STEP 1: CREATING A CHARACTER",
            "STEP 2: SETTING A STORY AND DESCRIPTION",
            "STEP 3: SETTING A SPECIAL",
            "STEP 4: SETTING TAG SKILLS",
            "STEP 5: ROLLING"
        };

        public static string[] TUTORIAL_CONTENTS = new string[]
        {
            "Use $char create [firstname] [lastname] to create your character.",
            "Use $char story set [story] and $char desc set [desc] to set your story and description.",
            "Use $char spec set [S] [P] [E] [C] [I] [A] [L] to set your SPECIAL.",
            "Use $char skills set [tag1] [tag2] [tag3] to set tag skills.",
            "Use $roll [special] and $roll [skill] to roll."
        };
        #endregion
        #endregion

        #region CHARACTER GROUP
        #region Display Character
        public static string[] CHAR_PAGE1_TITLES = new string[]
        {
            "Name",
            "Description",
            "Level",
            "Experience",
            "To Next Level",
            "Caps"
        };

        public static string[] CHAR_PAGE2_TITLES = new string[]
        {
            ""
        };

        public static string[] CHAR_PAGE1_CONTENTS = new string[]
        {

        };
        #endregion
        #endregion
    }
}
