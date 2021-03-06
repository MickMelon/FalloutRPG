﻿using FalloutRPG.Services.Roleplay;
using System;

namespace FalloutRPG.Constants
{
    public class Exceptions
    {
        // Character Exception Message
        public static readonly string CHAR_DISCORDID_EXISTS = "A character already exists with specified Discord ID.";
        public static readonly string CHAR_NAMES_NOT_LETTERS = "Character name must only consist of letters.";
        public static readonly string CHAR_NAMES_LENGTH = "Character name must be between 2-24 letters each.";
        public static readonly string CHAR_NAMES_NOT_UNIQUE = "Character name was not unique for your Discord ID.";
        public static readonly string CHAR_SPECIAL_LENGTH = "The input special length did not equal 7.";
        public static readonly string CHAR_SPECIAL_DOESNT_ADD_UP = "SPECIAL does not add up to 40.";
        public static readonly string CHAR_SPECIAL_NOT_IN_RANGE = "One or more SPECIAL attributes were not between {0} and {1}. ({2} attributes allowed at max during chargen).";
        public static readonly string CHAR_SPECIAL_NOT_FOUND = "Unable to find SPECIAL for that character.";
        public static readonly string CHAR_INVALID_TAG_NAMES = "One or more tag names were invalid.";
        public static readonly string CHAR_INVALID_SKILL_NAME = "Skill name was invalid.";
        public static readonly string CHAR_TAGS_NOT_UNIQUE = "One or more tag names were identical.";
        public static readonly string CHAR_CHARACTER_IS_NULL = "Character is null.";
        public static readonly string CHAR_NOT_ENOUGH_SKILL_POINTS = "Character does not have enough skill points.";
        public static readonly string CHAR_SKILL_POINTS_GOES_OVER_MAX = "Unable to add skill points because it will take the skill above the max level.";
        public static readonly string CHAR_SKILLS_NOT_SET = "Character skills aren't set.";
        public static readonly string CHAR_SPECIAL_NOT_SET = "Character SPECIAL isn't set.";
        public static readonly string CHAR_INVALID_SPECIAL_NAME = "SPECIAL name is invalid.";
        public static readonly string CHAR_NOT_ENOUGH_SPECIAL_POINTS = "Not enough SPECIAL points.";
        public static readonly string CHAR_SPECIAL_POINTS_GOES_OVER_MAX = "Unable to add SPECIAL points because it will take skill above the max level.";
        public static readonly string CHAR_TOO_MANY = "You have reached the limit of characters per account.";
        public static readonly string CHAR_TAGS_OUT_OF_RANGE = "Tag value was either too low or too high. (You may only have two skills set to 6.)";

        // NPC Exceptions
        public static readonly string NPC_CHAR_EXISTS = "A character with that name already exists!";
        public static readonly string NPC_LEVEL_TOO_HIGH = "The given level was too high for the NPC preset.";
        public static readonly string NPC_LEVEL_TOO_LOW = "The given level was too low for the NPC preset.";
        public static readonly string NPC_INVALID_PRESET = "The specified NPC preset was invalid.";
        public static readonly string NPC_INVALID_PRESET_DISABLED = "The specified NPC preset is not enabled yet.";
        public static readonly string NPC_PRESET_EXISTS = "The specified name matched an existing NPC preset";
    }
}
