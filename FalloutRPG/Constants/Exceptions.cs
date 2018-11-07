using System;

namespace FalloutRPG.Constants
{
    public class Exceptions
    {
        // Character Exception Message
        public const string CHAR_DISCORDID_EXISTS = "A character already exists with specified Discord ID.";
        public const string CHAR_NAMES_NOT_LETTERS = "Character name must only consist of letters.";
        public const string CHAR_NAMES_LENGTH = "Character name must be between 2-24 letters each.";
        public const string CHAR_NAMES_NOT_UNIQUE = "Character name was not unique for your Discord ID.";
        public const string CHAR_SPECIAL_LENGTH = "The input special length did not equal 7.";
        public const string CHAR_SPECIAL_DOESNT_ADD_UP = "SPECIAL does not add up to 40.";
        public const string CHAR_SPECIAL_NOT_IN_RANGE = "One or more SPECIAL attributes were not between 1 and 10.";
        public const string CHAR_SPECIAL_NOT_FOUND = "Unable to find SPECIAL for that character.";
        public const string CHAR_INVALID_TAG_NAMES = "One or more tag names were invalid.";
        public const string CHAR_INVALID_SKILL_NAME = "Skill name was invalid.";
        public const string CHAR_INVALID_STAT_NAME = "Given stat parameter did not match a S.P.E.C.I.A.L. stat or Skill.";
        public const string CHAR_TAGS_NOT_UNIQUE = "One or more tag names were identical.";
        public const string CHAR_CHARACTER_IS_NULL = "Character is null.";
        public const string CHAR_NOT_ENOUGH_SKILL_POINTS = "Character does not have enough skill points.";
        public const string CHAR_SKILL_POINTS_GOES_OVER_MAX = "Unable to add skill points because it will take the skill above the max level.";
        public const string CHAR_SKILLS_NOT_SET = "Character skills aren't set.";
        public const string CHAR_SPECIAL_NOT_SET = "Character SPECIAL isn't set.";
        public const string CHAR_INVALID_SPECIAL_NAME = "SPECIAL name is invalid.";
        public const string CHAR_NOT_ENOUGH_SPECIAL_POINTS = "Not enough SPECIAL points.";
        public const string CHAR_SPECIAL_POINTS_GOES_OVER_MAX = "Unable to add SPECIAL points because it will take skill above the max level.";
        public const string CHAR_TOO_MANY = "You have reached the limit of characters per account.";
        public const string CHAR_NOT_FOUND = "Unable to find character.";
        public const string CHAR_NOT_PLAYER = "The given character was not a player character.";

        // Campaign Exceptions
        public const string CAMP_TOO_MANY = "You have reached the limit of campaigns per account.";
        public const string CAMP_NAME_NOT_UNIQUE = "Campaign name was not unique for your server.";
        public const string CAMP_CHANNEL_COMMAND = "This command must be run in a campaign text channel.";
        public const string CAMP_NOT_FOUND = "Campaign not found.";
        public const string CAMP_NOT_OWNER = "This command can only be ran by the campaign owner.";
        public const string CAMP_NOT_MODERATOR = "This command can only be ran by a campaign moderator.";
        public const string CAMP_ALREADY_IN = "The user trying to be added is already the campaign.";
        public const string CAMP_NOT_A_MEMBER = "The specified player is not in this campaign.";
        public const string CAMP_NAME_LENGTH = "Campaign name must be between 2-24 letters each.";
        public const string CAMP_CHAR_NOT_JOINED = "The specified character is not part of this campaign.";

        // Campaign Cheat Exceptions/Other
        public const string LEVEL_TOO_LOW = "The specified level was too low.";

        // NPC Exceptions
        public const string NPC_CHAR_EXISTS = "A character with that name already exists!";
        public const string NPC_LEVEL_TOO_HIGH = "The given level was too high for the NPC preset.";
        public const string NPC_LEVEL_TOO_LOW = "The given level was too low for the NPC preset.";
        public const string NPC_INVALID_PRESET = "The specified NPC preset was invalid.";
        public const string NPC_INVALID_PRESET_DISABLED = "The specified NPC preset is not enabled yet.";

        // Roll Exceptions
        public const string ROLL_DICE_INVALID_STRING = "The given dice string was invalid. Example: 2d20+5";
        public const string ROLL_DICE_TOO_MANY = "There were too many die to be rolled; max is twenty.";
        public const string ROLL_DICE_TOO_MANY_SIDES = "There were too many sides on that die to be rolled; max is one hundred.";
    }
}
