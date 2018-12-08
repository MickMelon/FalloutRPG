namespace FalloutRPG.Constants
{
    public class Messages
    {
        public const string FAILURE_EMOJI = "\u274C ";
        public const string SUCCESS_EMOJI = "✅ ";
        public const string STAR_EMOJI = "\u2B50 ";
        public const string ADM_EMOJI = "👮 ";
        public const string QUESTION_EMOJI = "❓ ";
        public const string MUSCLE_EMOJI = "\uD83D\uDCAA";

        // Character Messages
        public const string CHAR_CREATED_SUCCESS = SUCCESS_EMOJI + "Character created successfully. ({0})";
        public const string CHAR_STORY_SUCCESS = SUCCESS_EMOJI + "Character story updated successfully. ({0})";
        public const string CHAR_DESC_SUCCESS = SUCCESS_EMOJI + "Character description updated successfully. ({0})";
        public const string CHAR_CHANGED_NAME = SUCCESS_EMOJI + "Character name successfully changed. ({0})";
        public const string CHAR_ACTIVATED = SUCCESS_EMOJI + "Character {0} is now active. ({1})";
        public const string CHAR_REMOVE_CONFIRM = QUESTION_EMOJI + "**Are you sure you want to delete `{0}` (level {1})? This action CANNOT be undone!**" +
            " To confirm this action, reply with the name of the character. ({2})";
        public const string CHAR_REMOVE_SUCCESS = SUCCESS_EMOJI + "The character `{0}` was deleted successfully. ({1})";
        public const string CHAR_NOT_REMOVED = FAILURE_EMOJI + "The character `{0}` was **NOT** deleted. ({1})";

        // Stats Messages
        public const string EXP_LEVEL_UP = "Congratulations {0}, you have just advanced to level {1}!";
        public const string SKILLS_LEVEL_UP = "Hey, {0}! You have {1} unspent skill points. Spend them with *!char skills spend [skill] [points]*";
        public const string SKILLS_SPEND_POINTS_SUCCESS = SUCCESS_EMOJI + "Skill points added successfully. ({0})";
        public const string SKILLS_SET_SUCCESS = SUCCESS_EMOJI + "Character skills set successfully. ({0})";
        public const string SPECIAL_SET_SUCCESS = SUCCESS_EMOJI + "Character SPECIAL set successfully. ({0})";
        public const string SKILLS_POINTS_CLAIMED = SUCCESS_EMOJI + "{0} skill points were successfully claimed! ({1})";

        // Money Messages
        public const string PAY_SUCCESS = SUCCESS_EMOJI + "You gave {0} {1} caps. ({2})";
        public const string ERR_NOT_ENOUGH_CAPS = FAILURE_EMOJI + "You do not have enough caps! ({0})";

        // Admin Messages
        public const string ADM_GAVE_MONEY = ADM_EMOJI + "Money given successfully. ({0})";
        public const string ADM_GAVE_SKILL_POINTS = ADM_EMOJI + "Skill points given successfully. ({0})";
        public const string ADM_GAVE_EXP_POINTS = ADM_EMOJI + "Experience points given successfully. ({0})";
        public const string ADM_GAVE_SPEC_POINTS = ADM_EMOJI + "SPECIAL points given successfully. ({0})";
        public const string ADM_RESET = ADM_EMOJI + "Reset character skills and SPECIAL successfully. ({0})";
        public const string ADM_DELETE = ADM_EMOJI + "Deleted character successfully. ({0})";
        public const string ADM_CHANGED_NAME = ADM_EMOJI + "Character name changed successfully. ({0})";

        // Command Error Messages
        public const string ERR_CMD_USAGE = FAILURE_EMOJI + "Incorrect command usage. Use $help if you are stuck. ({0})";
        public const string ERR_CMD_NOT_EXIST = FAILURE_EMOJI + "Command doesn't exist. Use $help if you are stuck. ({0})";

        // Character Error Messages
        public const string ERR_CHAR_NOT_FOUND = FAILURE_EMOJI + "Unable to find character. ({0})";
        public const string ERR_STORY_NOT_FOUND = FAILURE_EMOJI + "Unable to find character story. ({0})";
        public const string ERR_DESC_NOT_FOUND = FAILURE_EMOJI + "Unable to find character description. ({0})";
        public const string ERR_SPECIAL_NOT_FOUND = FAILURE_EMOJI + "Unable to find character SPECIAL. ({0})";
        public const string ERR_CHAR_ALREADY_ACTIVE = FAILURE_EMOJI + "Character `{0}` is already active. ({1})";
        public const string ERR_CHAR_CANT_REMOVE_ACTIVE = FAILURE_EMOJI + "Character `{0}` cannot be removed because it is your active character. " +
            "Please switch by using the `$char activate [name]` command. ({1})";

        // Stats Error Messages
        public const string ERR_SKILLS_NOT_FOUND = FAILURE_EMOJI + "Unable to find character skills. ({0})";
        public const string ERR_SKILLS_ALREADY_SET = FAILURE_EMOJI + "Character skills are already set. ({0})";
        public const string ERR_SPECIAL_ALREADY_SET = FAILURE_EMOJI + "Character SPECIAL is already set. ({0})";
        public const string ERR_SKILLS_NONE_TO_CLAIM = FAILURE_EMOJI + "You don't have any skill points that you can claim. ({0})";
        public const string ERR_SKILLS_POINTS_BELOW_ONE = FAILURE_EMOJI + "You cannot put less than one point in a skill. ({0})";

        // Gambling Messages
        public const string BET_PLACED = SUCCESS_EMOJI + "{0}, bet placed!";

        // Gambling Error Messages
        public const string ERR_BALANCE_ADD_FAIL = FAILURE_EMOJI + "{0}, I failed to add your balance to the directory! (Do you have a character?)";
        public const string ERR_BET_TOO_HIGH = FAILURE_EMOJI + "{0}, that bet is too high! (Do you have enough money?)";
        public const string ERR_BET_TOO_LOW = FAILURE_EMOJI + "{0}, that bet is too low!";

        // Craps Messages
        public const string CRAPS_CRAPOUT = "{0} crapped out!";
        public const string CRAPS_CRAPOUT_POS = "{0} crapped out, but they were counting on it!";
        public const string CRAPS_CRAPOUT_PUSH = "{0} crapped out, but the House always gets the best odds!";
        public const string CRAPS_NATURAL = "{0} rolled a Natural!";
        public const string CRAPS_NATURAL_NEG = "{0} rolled a Natural but they wish they hadn't!";
        public const string CRAPS_POINT_ROUND = "{0} advanced the round into the Point!";
        public const string CRAPS_POINT_SET = "{0}'s point is {1}";
        public const string CRAPS_POINT_ROLL = "{0} rolled their point!";
        public const string CRAPS_POINT_ROLL_NEG = "{0} rolled their point, but they betted against it!";
        public const string CRAPS_SEVEN_OUT = "{0} sevened out!";
        public const string CRAPS_SEVEN_OUT_POS = "{0} sevened out, but they were counting on it!";
        public const string CRAPS_NEW_SHOOTER = "{0} is the new shooter.";
        public const string CRAPS_NEW_SHOOTER_ONE_PLAYER = "{0} is the \"new\" shooter since no one else is playing!";
        public const string CRAPS_NEW_MATCH = "A new match of Craps is starting, with {0} as the first shooter!";
        public const string CRAPS_JOIN_MATCH = "{0} joins the match!";
        public const string CRAPS_ALREADY_IN_MATCH = "{0}, you're already in this match!";
        public const string CRAPS_LEAVE_MATCH = "{0} left the match.";
        public const string CRAPS_EMPTY_MATCH = "{0}, to roll you must join the match first.";
        public const string CRAPS_INACTIVITY_ROLL = "Rolling for {0} due to inactivity.";
        public const string CRAPS_INACTIVITY_KICK = "{0} has been removed from the match due to inactivity.";
        public const string CRAPS_INACTIVITY_PASS_DICE = "Gave the dice to {0} since {1} was inactive.";

        // Craps Error Messages
        public const string ERR_CRAPS_NOT_SHOOTER = FAILURE_EMOJI + "{0}, {1} is the current shooter. (Join the match and wait your turn.)";
        public const string ERR_CRAPS_BET_NOT_SET = FAILURE_EMOJI + "{0}, you do not have a bet placed!";
        public const string ERR_CRAPS_BET_ALREADY_SET = FAILURE_EMOJI + "{0}, you already have a bet!";
        public const string ERR_CRAPS_BET_PARSE_FAIL = FAILURE_EMOJI + "{0}, valid bet types are: 'pass', 'dontpass', 'come', or 'dontcome' (without single quotes.)";
        public const string ERR_CRAPS_POINT_NOT_SET = FAILURE_EMOJI + "{0}, you can't place a Come bet when the Point isn't set!";
        public const string ERR_CRAPS_POINT_ALREADY_SET = FAILURE_EMOJI + "{0}, you can't place a Pass bet after the Point has been set!";
        public const string ERR_CRAPS_JOIN_FAIL = FAILURE_EMOJI + "Failed to join {0} into the match!";
        public const string ERR_CRAPS_LEAVE_FAIL = FAILURE_EMOJI + "Failed to remove {0} match, are they the shooter?";
        public const string ERR_CRAPS_PASS_FAIL = FAILURE_EMOJI + "Couldn't pass the dice for {0}. (Do you have a bet placed, or the only one playing?)";

        // Effects Messages
        public const string EFFECT_CREATE_SUCCESS = SUCCESS_EMOJI + "Effect created successfully. ({0})";
        public const string EFFECT_EDIT_SUCCESS = SUCCESS_EMOJI + "Effect edited successfully. ({0})";
        public const string EFFECT_DELETE_SUCCESS = SUCCESS_EMOJI + "Effect deleted successfully. ({0})";
        public const string EFFECT_APPLY_SUCCESS = SUCCESS_EMOJI + "`{0}` applied successfully to `{1}`. ({2})";
        public const string EFFECT_REMOVE_SUCCESS = SUCCESS_EMOJI + "`{0}` successfully removed from `{1}`. ({2})";

        // Effects Error Messages
        public const string ERR_EFFECT_NOT_FOUND = FAILURE_EMOJI + "Unable to find effect. ({0})";
        public const string ERR_EFFECT_TOO_MANY = FAILURE_EMOJI + "You have reached the limit of effects per account. ({0})";
        public const string ERR_EFFECT_NAME_DUPLICATE = FAILURE_EMOJI + "An effect already exists with the given name. ({0})";
        public const string ERR_EFFECT_NAME_TOO_LONG = FAILURE_EMOJI + "The given effect name was too long. ({0})";
        public const string ERR_EFFECT_NOT_ALPHABETICAL = FAILURE_EMOJI + "The effect name must contain only letters. ({0})";
        public const string ERR_EFFECT_ALREADY_APPLIED = FAILURE_EMOJI + "The specified effect is already applied. ({0})";
    }
}
