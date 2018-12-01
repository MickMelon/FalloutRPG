namespace FalloutRPG.Constants
{
    public class Messages
    {
        public const string FAILURE_EMOJI = "\u274C ";
        public const string SUCCESS_EMOJI = "✅ ";
        public const string STAR_EMOJI = "\u2B50 ";
        public const string ADM_EMOJI = "👮 ";
        public const string NPC_SUFFIX = "(\uD83D\uDCBBNPC) ";
        public const string QUESTION_EMOJI = "❓ ";
        public const string DICE_EMOJI = "\uD83C\uDFB2";
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

        // NPC Messages
        public const string NPC_CREATED_SUCCESS = SUCCESS_EMOJI + "NPC created with type: {0} and name: {1}";
        public const string NPC_CANT_USE_SKILL = FAILURE_EMOJI + "{0} can't use this skill! " + NPC_SUFFIX;
        public const string NPC_CANT_USE_SPECIAL = FAILURE_EMOJI + "{0} can't use this S.P.E.C.I.A.L. stat! " + NPC_SUFFIX;
        public const string NPC_CANT_USE_STAT = FAILURE_EMOJI + "{0} can't use this attribute! " + NPC_SUFFIX;
        public const string NPC_PRESET_CREATE = SUCCESS_EMOJI + "NPC preset: {0} created successfully. ({1})";
        public const string NPC_PRESET_TOGGLE = SUCCESS_EMOJI + "NPC preset: {0} enabled status: {1} ({2})";
        public const string NPC_PRESET_EDIT_TAGS = SUCCESS_EMOJI + "NPC preset: {0} Tags changed. ({1})";
        public const string NPC_PRESET_EDIT_INVENTORY = SUCCESS_EMOJI + "NPC preset: {0} initial inventory changed. ({1})";
        public const string NPC_PRESET_EDIT_SPECIAL = SUCCESS_EMOJI + "NPC preset: {0} S.P.E.C.I.A.L. values changed. ({1})";

        // NPC Error Messages
        public const string ERR_NPC_CHAR_NOT_FOUND = FAILURE_EMOJI + "An NPC with the name {0} could not be found.";
        public const string ERR_NPC_PRESET_NOT_FOUND = FAILURE_EMOJI + "An NPC preset with the name {0} could not be found. ({1})";
        public const string ERR_NPC_PRESET_CREATE = FAILURE_EMOJI + "NPC preset creation failed. ({0})";
        public const string ERR_NPC_PRESET_ENABLE = FAILURE_EMOJI + "Failed to enable NPC preset: {0}. (Check for typos) ({1})";
        public const string ERR_NPC_PRESET_DISABLE = FAILURE_EMOJI + "Failed to disable NPC preset: {0}. (Check for typos) ({1})";
        public const string ERR_NPC_PRESET_EDIT = FAILURE_EMOJI + "Failed to modify NPC preset attribute. ({0})";


        // Item Messages
        public const string ITEM_GIVE_SUCCESS = SUCCESS_EMOJI + "Gave item {0} to {1}. ({2})";
        public const string ITEM_CREATE_SUCCESS = SUCCESS_EMOJI + "Created new item with name {0} and type {1}. ({2})";

        // Item Error Messages
        public const string ERR_ITEM_NOT_FOUND = FAILURE_EMOJI + "That item was unable to be found. ({0})";
        public const string ERR_ITEM_INVALID_SLOT = FAILURE_EMOJI + "The given apparel slot was invalid. ({0})";
        public const string ERR_ITEM_INVALID_AMMO = FAILURE_EMOJI + "The given ammo was unable to be found; does it exist yet? ({0})";

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

        // Roll Messages
        public const string ROLL_DICE = DICE_EMOJI + "Rolled: {0}. ({1})";
    }
}
