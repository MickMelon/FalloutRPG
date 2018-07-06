﻿using System;

namespace FalloutRPG.Constants
{
    public class Exceptions
    {
        // Character Exception Message
        public const string CHAR_DISCORDID_EXISTS = "A character already exists with specified Discord ID.";
        public const string CHAR_NAMES_NOT_LETTERS = "Both character names must only consist of letters.";
        public const string CHAR_NAMES_LENGTH = "Both character names must be between 2-24 letters each.";
        public const string CHAR_SPECIAL_LENGTH = "The input special length did not equal 7.";
        public const string CHAR_SPECIAL_DOESNT_ADD_UP = "SPECIAL does not add up to 40.";
        public const string CHAR_SPECIAL_NOT_IN_RANGE = "One or more SPECIAL attributes were not between 1 and 10.";
        public const string CHAR_SPECIAL_NOT_FOUND = "Unable to find SPECIAL for that character.";
        public const string CHAR_INVALID_TAG_NAMES = "One or more tag names were invalid.";
        public const string CHAR_INVALID_SKILL_NAME = "Skill name was invalid.";
        public const string CHAR_TAGS_NOT_UNIQUE = "One or more tag names were identical.";
        public const string CHAR_CHARACTER_IS_NULL = "Character is null.";
        public const string CHAR_NOT_ENOUGH_SKILL_POINTS = "Character does not have enough skill points.";
        public const string CHAR_SKILL_POINTS_GOES_OVER_MAX = "Unable to add skill points because it will take the skill above the max level.";
        public const string CHAR_SKILLS_NOT_SET = "Character skills aren't set.";
    }
}
