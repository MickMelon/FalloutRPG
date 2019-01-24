using System.Collections.ObjectModel;

namespace FalloutRPG.Constants
{
    public class Pages
    {
        #region HELP GROUP
        #region Index Help Pages
        public static ReadOnlyCollection<string> HELP_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[]
        {
            "$help general",
            "$help character",
            "$help roll",
            "$help skills",
            "$help craps"
        });

        public static ReadOnlyCollection<string> HELP_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Displays general help menu.",
            "Displays character help menu.",
            "Displays roll help menu.",
            "Displays a list of skills.",
            "Displays craps help page."
        });
        #endregion

        #region General Help Pages
        public static ReadOnlyCollection<string> HELP_GENERAL_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "$pay [@user] [amount]",
            "$daysleft",
            "$highscores"
        });

        public static ReadOnlyCollection<string> HELP_GENERAL_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Pays a user the specified amount of caps.",
            "Shows how many days are left until the release of Fallout 76 using UTC.",
            "Shows the top 10 characters ordered by experience."
        });
        #endregion

        #region Character Help Pages
        public static ReadOnlyCollection<string> HELP_CHAR_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
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
        });

        public static ReadOnlyCollection<string> HELP_CHAR_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
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
        });

        public static ReadOnlyCollection<string> HELP_CHAR_PAGE2_TITLES = new ReadOnlyCollection<string>(new string[] 
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
        });

        public static ReadOnlyCollection<string> HELP_CHAR_PAGE2_CONTENTS = new ReadOnlyCollection<string>(new string[] 
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
        });
        #endregion

        #region NPC Help Pages
        public static ReadOnlyCollection<string> HELP_NPC_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "$npc create [NPC type] [First Name]",
            "$npc roll [Skill/SPECIAL to roll] [First name]"
        });
        public static ReadOnlyCollection<string> HELP_NPC_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Creates a new NPC with slightly random stats influenced by the given NPC type.",
            "Gets a roll result based on the NPC's skills or S.P.E.C.I.A.L.."
        });
        public static ReadOnlyCollection<string> HELP_NPC_PRESETS_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "$npc preset create [Name]",
            "$npc preset create [Name] [STR] [PER] [END] [CHA] [INT] [AGI] [LUC]",
            "$npc preset enable [Name]",
            "$npc preset disable [Name]",
            "$npc preset edit [Name] [Attribute] [Value]",
            "$npc preset edit [Name] [STR] [PER] [END] [CHA] [INT] [AGI] [LUC]",
            "$npc preset init",
            "$npc preset view",
        });
        public static ReadOnlyCollection<string> HELP_NPC_PRESETS_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Creates a new NPC preset with the given name. Note: this preset will be completely empty.",
            "Creates a new NPC preset with the given name and S.P.E.C.I.A.L.. This preset will have its Skills set to what a player's would look like without 'Tag!'.",
            "Enables the preset with the given name. This command must be run before users are able to create NPCs with that preset.",
            "Disables the preset with the given name. Users will no longer be able to create NPCs with the given preset after execution.",
            "Edits the specified attribute of the preset with the given name. 'Attribute' can be the name of a S.P.E.C.I.A.L. stat or Skill.",
            "Edits the S.P.E.C.I.A.L. of the preset with the given name. Helpful for when you don't want to type the same command 7 times.",
            "Sets the NPC preset's Skills to what they'd look like if it were a new player's without Tag!.",
            "Sends a DM to you with all of the NPC preset's current properties (Skills, S.P.E.C.I.A.L., other stuff...)"
        });
        #endregion

        #region Roll Help Pages
        public static ReadOnlyCollection<string> HELP_ROLL_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "$roll [skill]",
            "$roll [special]"
        });

        public static ReadOnlyCollection<string> HELP_ROLL_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Gets a roll result based on the skill level.",
            "Gets a roll result based on the SPECIAL level."
        });
        #endregion

        #region Craps Help Pages
        public static ReadOnlyCollection<string> HELP_CRAPS_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "$craps join",
            "$craps leave",
            "$craps roll",
            "$craps bet [type] [amount]",
            "$craps pass",
            "Bet Types"
        });

        public static ReadOnlyCollection<string> HELP_CRAPS_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Joins current craps game.",
            "Leaves current craps game.",
            "Rolls the dice.",
            "Makes a bet of the type and amount. Types can be found below.",
            "Pass the dice to another user.",
            "pass, dontpass, come, dontcome"
        });
        #endregion

        #region Admin Help Pages
        public static ReadOnlyCollection<string> HELP_ADMIN_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "$admin givemoney [@user] [amount]",
            "$admin giveskillpoints [@user] [amount]",
            "$admin changename [@user] [name]",
            "$admin reset [@user]"
        });

        public static ReadOnlyCollection<string> HELP_ADMIN_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Gives a character the specified amount of caps.",
            "Gives a character the specified amount of skill points.",
            "Changes a character's name.",
            "Resets a character's skill points and SPECIAL. They will then be able to use *$char skills claim* to claim their skill points back."
        });
        #endregion

        #region Tutorial
        public static ReadOnlyCollection<string> TUTORIAL_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "STEP 1: CREATING A CHARACTER",
            "STEP 2: SETTING A STORY AND DESCRIPTION",
            "STEP 3: SETTING A SPECIAL",
            "STEP 4: SETTING TAG SKILLS",
            "STEP 5: ROLLING"
        });

        public static ReadOnlyCollection<string> TUTORIAL_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {
            "Use $char create [firstname] [lastname] to create your character.",
            "Use $char story set [story] and $char desc set [desc] to set your story and description.",
            "Use $char spec set [S] [P] [E] [C] [I] [A] [L] to set your SPECIAL.",
            "Use $char skills set [tag1] [tag2] [tag3] to set tag skills.",
            "Use $roll [special] and $roll [skill] to roll."
        });
        #endregion
        #endregion

        #region CHARACTER GROUP
        #region Display Character
        public static ReadOnlyCollection<string> CHAR_PAGE1_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            "Name",
            "Description",
            "Level",
            "Experience",
            "To Next Level",
            "Caps"
        });

        public static ReadOnlyCollection<string> CHAR_PAGE2_TITLES = new ReadOnlyCollection<string>(new string[] 
        {
            ""
        });

        public static ReadOnlyCollection<string> CHAR_PAGE1_CONTENTS = new ReadOnlyCollection<string>(new string[] 
        {

        });
        #endregion
        #endregion
    }
}
