﻿namespace FalloutRPG.Constants
{
    public class Messages
    {
        public static readonly string FAILURE_EMOJI = "\u274C ";
        public static readonly string SUCCESS_EMOJI = "✅ ";
        public static readonly string STAR_EMOJI = "\u2B50 ";
        public static readonly string ADM_EMOJI = "👮 ";
        public static readonly string NPC_SUFFIX = "\uD83D\uDCBBNPC ";
        public static readonly string QUESTION_EMOJI = "❓ ";
        public static readonly string MUSCLE_EMOJI = "\uD83D\uDCAA";
        public static readonly string DICE_EMOJI = "\uD83C\uDFB2";

        // Character Messages
        public static readonly string CHAR_CREATED_SUCCESS = SUCCESS_EMOJI + "Character created successfully.";
        public static readonly string CHAR_STORY_SUCCESS = SUCCESS_EMOJI + "Character story updated successfully.";
        public static readonly string CHAR_DESC_SUCCESS = SUCCESS_EMOJI + "Character description updated successfully.";
        public static readonly string CHAR_CHANGED_NAME = SUCCESS_EMOJI + "Character name successfully changed.";
        public static readonly string CHAR_ACTIVATED = SUCCESS_EMOJI + "Character {0} is now active.";
        public static readonly string CHAR_REMOVE_CONFIRM = QUESTION_EMOJI + "**Are you sure you want to delete `{0}` level {1}? This action CANNOT be undone!**" +
            " To confirm this action, reply with the name of the character.";
        public static readonly string CHAR_REMOVE_SUCCESS = SUCCESS_EMOJI + "The character `{0}` was deleted successfully.";
        public static readonly string CHAR_NOT_REMOVED = FAILURE_EMOJI + "The character `{0}` was **NOT** deleted.";

        // Stats Messages
        public static readonly string EXP_LEVEL_UP = "Congratulations {0}, you have just advanced to level {1}!";
        public static readonly string SKILLS_LEVEL_UP = "You have {0} unspent skill points. Spend them with *$char skills spend [skill] [points]*";
        public static readonly string SKILLS_SPEND_POINTS_SUCCESS = SUCCESS_EMOJI + "Skill points added successfully.";
        public static readonly string SKILLS_SET_SUCCESS = SUCCESS_EMOJI + "Character skills set successfully.";
        public static readonly string SPECIAL_SET_SUCCESS = SUCCESS_EMOJI + "Character SPECIAL set successfully.";
        public static readonly string SKILLS_POINTS_CLAIMED = SUCCESS_EMOJI + "{0} skill points were successfully claimed!";
        public static readonly string SKILLS_ADDED = SUCCESS_EMOJI + "Skill successfully added.";
        public static readonly string SKILLS_REMOVED = SUCCESS_EMOJI + "Skill successfully removed.";
        public static readonly string SPECIAL_ADDED = SUCCESS_EMOJI + "Special successfully added.";
        public static readonly string SPECIAL_REMOVED = SUCCESS_EMOJI + "Special successfully removed.";
        public static readonly string STAT_RENAMED = SUCCESS_EMOJI + "Statistic renamed successfully.";

        // Money Messages
        public static readonly string PAY_SUCCESS = SUCCESS_EMOJI + "You gave {0} {1} caps.";
        public static readonly string ERR_NOT_ENOUGH_CAPS = FAILURE_EMOJI + "You do not have enough caps!";

        // Admin Messages
        public static readonly string ADM_GAVE_MONEY = ADM_EMOJI + "Money given successfully.";
        public static readonly string ADM_GAVE_SKILL_POINTS = ADM_EMOJI + "Skill points given successfully.";
        public static readonly string ADM_GAVE_EXP_POINTS = ADM_EMOJI + "Experience points given successfully.";
        public static readonly string ADM_GAVE_SPEC_POINTS = ADM_EMOJI + "SPECIAL points given successfully.";
        public static readonly string ADM_RESET = ADM_EMOJI + "Reset character skills and SPECIAL successfully.";
        public static readonly string ADM_DELETE = ADM_EMOJI + "Deleted character successfully.";
        public static readonly string ADM_CHANGED_NAME = ADM_EMOJI + "Character name changed successfully.";

        // Command Error Messages
        public static readonly string ERR_CMD_USAGE = FAILURE_EMOJI + "Incorrect command usage. Use $help if you are stuck.";
        public static readonly string ERR_CMD_NOT_EXIST = FAILURE_EMOJI + "Command doesn't exist. Use $help if you are stuck.";
        public static readonly string ERR_CMD_REQUIRE_ROLE_FAIL = FAILURE_EMOJI + "You must be have the {0} role to use this command.";
        public static readonly string ERR_CMD_NOT_IN_GUILD = FAILURE_EMOJI + "You must be in a server to use this command.";

        // Character Error Messages
        public static readonly string ERR_CHAR_NOT_FOUND = FAILURE_EMOJI + "Unable to find character.";
        public static readonly string ERR_STORY_NOT_FOUND = FAILURE_EMOJI + "Unable to find character story.";
        public static readonly string ERR_DESC_NOT_FOUND = FAILURE_EMOJI + "Unable to find character description.";
        public static readonly string ERR_SPECIAL_NOT_FOUND = FAILURE_EMOJI + "Unable to find character SPECIAL.";
        public static readonly string ERR_CHAR_ALREADY_ACTIVE = FAILURE_EMOJI + "Character `{0}` is already active.";
        public static readonly string ERR_CHAR_CANT_REMOVE_ACTIVE = FAILURE_EMOJI + "Character `{0}` cannot be removed because it is your active character. " +
            "Please switch by using the `$char activate [name]` command.";

        // Stats Error Messages
        public static readonly string ERR_SKILLS_NOT_FOUND = FAILURE_EMOJI + "Unable to find character skills.";
        public static readonly string ERR_SKILLS_ALREADY_SET = FAILURE_EMOJI + "Character skills are already set.";
        public static readonly string ERR_SKILLS_POINTS_GOES_OVER_MAX = "Unable to add skill points because it will take the skill above the max level.";
        public static readonly string ERR_SPECIAL_ALREADY_SET = FAILURE_EMOJI + "Character SPECIAL is already set.";
        public static readonly string ERR_SKILLS_NONE_TO_CLAIM = FAILURE_EMOJI + "You don't have any skill points that you can claim.";
        public static readonly string ERR_SKILLS_POINTS_BELOW_ONE = FAILURE_EMOJI + "You cannot put less than one point in a skill.";
        public static readonly string ERR_SKILLS_TOO_LOW = FAILURE_EMOJI + "The skill's level was too low to use.";
        public static readonly string ERR_STAT_ALREADY_EXISTS = FAILURE_EMOJI + "A statistic with the given name already exists.";
        public static readonly string ERR_STAT_NOT_ENOUGH_POINTS = "Character does not have enough experience points. Price: {0}XP.";
        public static readonly string ERR_STAT_PRICE_NOT_SET = "Upgrade price not set.";
        public static readonly string ERR_STAT_USING_OLD_PROGRESSION = "Configuration is set to use old progresion system.";
        public static readonly string ERR_STAT_NOT_USING_OLD_PROGRESSION = "Configuration is not set to use old progresion system.";
        public static readonly string ERR_STAT_NOT_USING_NEW_VEGAS_RULES = "Configuration is not set to use the Fallout: New Vegas ruleset.";

        // Gambling Messages
        public static readonly string BET_PLACED = SUCCESS_EMOJI + "{0}, bet placed!";

        // Gambling Error Messages
        public static readonly string ERR_BALANCE_ADD_FAIL = FAILURE_EMOJI + "{0}, I failed to add your balance to the directory! Do you have a character?";
        public static readonly string ERR_BET_TOO_HIGH = FAILURE_EMOJI + "{0}, that bet is too high! Do you have enough money?";
        public static readonly string ERR_BET_TOO_LOW = FAILURE_EMOJI + "{0}, that bet is too low!";

        // Craps Messages
        public static readonly string CRAPS_CRAPOUT = "{0} crapped out!";
        public static readonly string CRAPS_CRAPOUT_POS = "{0} crapped out, but they were counting on it!";
        public static readonly string CRAPS_CRAPOUT_PUSH = "{0} crapped out, but the House always gets the best odds!";
        public static readonly string CRAPS_NATURAL = "{0} rolled a Natural!";
        public static readonly string CRAPS_NATURAL_NEG = "{0} rolled a Natural but they wish they hadn't!";
        public static readonly string CRAPS_POINT_ROUND = "{0} advanced the round into the Point!";
        public static readonly string CRAPS_POINT_SET = "{0}'s point is {1}";
        public static readonly string CRAPS_POINT_ROLL = "{0} rolled their point!";
        public static readonly string CRAPS_POINT_ROLL_NEG = "{0} rolled their point, but they betted against it!";
        public static readonly string CRAPS_SEVEN_OUT = "{0} sevened out!";
        public static readonly string CRAPS_SEVEN_OUT_POS = "{0} sevened out, but they were counting on it!";
        public static readonly string CRAPS_NEW_SHOOTER = "{0} is the new shooter.";
        public static readonly string CRAPS_NEW_SHOOTER_ONE_PLAYER = "{0} is the \"new\" shooter since no one else is playing!";
        public static readonly string CRAPS_NEW_MATCH = "A new match of Craps is starting, with {0} as the first shooter!";
        public static readonly string CRAPS_JOIN_MATCH = "{0} joins the match!";
        public static readonly string CRAPS_ALREADY_IN_MATCH = "{0}, you're already in this match!";
        public static readonly string CRAPS_LEAVE_MATCH = "{0} left the match.";
        public static readonly string CRAPS_EMPTY_MATCH = "{0}, to roll you must join the match first.";
        public static readonly string CRAPS_INACTIVITY_ROLL = "Rolling for {0} due to inactivity.";
        public static readonly string CRAPS_INACTIVITY_KICK = "{0} has been removed from the match due to inactivity.";
        public static readonly string CRAPS_INACTIVITY_PASS_DICE = "Gave the dice to {0} since {1} was inactive.";

        // Craps Error Messages
        public static readonly string ERR_CRAPS_NOT_SHOOTER = FAILURE_EMOJI + "{0}, {1} is the current shooter. Join the match and wait your turn.";
        public static readonly string ERR_CRAPS_BET_NOT_SET = FAILURE_EMOJI + "{0}, you do not have a bet placed!";
        public static readonly string ERR_CRAPS_BET_ALREADY_SET = FAILURE_EMOJI + "{0}, you already have a bet!";
        public static readonly string ERR_CRAPS_BET_PARSE_FAIL = FAILURE_EMOJI + "{0}, valid bet types are: 'pass', 'dontpass', 'come', or 'dontcome' without single quotes.";
        public static readonly string ERR_CRAPS_POINT_NOT_SET = FAILURE_EMOJI + "{0}, you can't place a Come bet when the Point isn't set!";
        public static readonly string ERR_CRAPS_POINT_ALREADY_SET = FAILURE_EMOJI + "{0}, you can't place a Pass bet after the Point has been set!";
        public static readonly string ERR_CRAPS_JOIN_FAIL = FAILURE_EMOJI + "Failed to join {0} into the match!";
        public static readonly string ERR_CRAPS_LEAVE_FAIL = FAILURE_EMOJI + "Failed to remove {0} match, are they the shooter?";
        public static readonly string ERR_CRAPS_PASS_FAIL = FAILURE_EMOJI + "Couldn't pass the dice for {0}. Do you have a bet placed, or the only one playing?";

        // Effects Messages
        public static readonly string EFFECT_CREATE_SUCCESS = SUCCESS_EMOJI + "Effect created successfully.";
        public static readonly string EFFECT_EDIT_SUCCESS = SUCCESS_EMOJI + "Effect edited successfully.";
        public static readonly string EFFECT_DELETE_SUCCESS = SUCCESS_EMOJI + "Effect deleted successfully.";
        public static readonly string EFFECT_APPLY_SUCCESS = SUCCESS_EMOJI + "`{0}` applied successfully to `{1}`.";
        public static readonly string EFFECT_REMOVE_SUCCESS = SUCCESS_EMOJI + "`{0}` successfully removed from `{1}`.";

        // Effects Error Messages
        public static readonly string ERR_EFFECT_NOT_FOUND = FAILURE_EMOJI + "Unable to find effect.";
        public static readonly string ERR_EFFECT_TOO_MANY = FAILURE_EMOJI + "You have reached the limit of effects per account.";
        public static readonly string ERR_EFFECT_NAME_DUPLICATE = FAILURE_EMOJI + "An effect already exists with the given name.";
        public static readonly string ERR_EFFECT_NAME_TOO_LONG = FAILURE_EMOJI + "The given effect name was too long.";
        public static readonly string ERR_EFFECT_NOT_ALPHABETICAL = FAILURE_EMOJI + "The effect name must contain only letters.";
        public static readonly string ERR_EFFECT_ALREADY_APPLIED = FAILURE_EMOJI + "The specified effect is already applied.";

        // NPC Messages
        public static readonly string NPC_CREATED_SUCCESS = SUCCESS_EMOJI + "NPC created with type: `{0}` and name: `{1}`.";
        public static readonly string NPC_CANT_USE_SKILL = FAILURE_EMOJI + "`{0}` can't use this skill! " + NPC_SUFFIX;
        public static readonly string NPC_CANT_USE_SPECIAL = FAILURE_EMOJI + "`{0}` can't use this S.P.E.C.I.A.L. stat! " + NPC_SUFFIX;
        public static readonly string NPC_CANT_USE_STAT = FAILURE_EMOJI + "`{0}` can't use this attribute! " + NPC_SUFFIX;
        public static readonly string NPC_PRESET_CREATE = SUCCESS_EMOJI + "NPC preset: `{0}` created successfully. {1}";
        public static readonly string NPC_PRESET_TOGGLE = SUCCESS_EMOJI + "NPC preset: `{0}` is Enabled: `{1}`. {2}";
        public static readonly string NPC_PRESET_EDIT_TAGS = SUCCESS_EMOJI + "NPC preset: `{0}` Tags sucessfully changed. {1}";
        public static readonly string NPC_PRESET_EDIT_INVENTORY = SUCCESS_EMOJI + "NPC preset: `{0}` initial inventory changed. {1}";
        public static readonly string NPC_PRESET_EDIT_SPECIAL = SUCCESS_EMOJI + "NPC preset: `{0}` S.P.E.C.I.A.L. values changed. {1}";

        // NPC Error Messages
        public static readonly string ERR_NPC_CHAR_NOT_FOUND = FAILURE_EMOJI + "An NPC with the given name could not be found.";
        public static readonly string ERR_NPC_PRESET_NOT_FOUND = FAILURE_EMOJI + "An NPC preset with the given name could not be found.";
        public static readonly string ERR_NPC_PRESET_CREATE = FAILURE_EMOJI + "NPC preset creation failed.";
        public static readonly string ERR_NPC_PRESET_ENABLE = FAILURE_EMOJI + "Failed to enable NPC preset: {0}. Check for typos.";
        public static readonly string ERR_NPC_PRESET_DISABLE = FAILURE_EMOJI + "Failed to disable NPC preset: {0}. Check for typos.";
        public static readonly string ERR_NPC_PRESET_EDIT = FAILURE_EMOJI + "Failed to modify NPC preset attribute.";

        // Roll Messages
        public static readonly string ROLL_DICE = DICE_EMOJI + "Rolled: {0}.";

        // Roll Error Messages
        public static readonly string ROLL_DICE_TOO_MANY = "There were too many die to be rolled; max is twenty.";
        public static readonly string ROLL_DICE_TOO_MANY_SIDES = "There were too many sides on that die to be rolled; max is one hundred.";
    }
}
