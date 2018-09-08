using FalloutRPG.Constants;
using FalloutRPG.Models;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class SkillsService
    {
        private const int DEFAULT_SKILL_POINTS = 10;
        public const int MAX_SKILL_LEVEL = 200;

        private readonly CharacterService _charService;
        private readonly SpecialService _specService;

        public SkillsService(CharacterService charService, SpecialService specService)
        {
            _charService = charService;
            _specService = specService;
        }

        /// <summary>
        /// Set character's tag skills.
        /// </summary>
        public async Task SetTagSkills(Character character, string tag1, string tag2, string tag3)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!_specService.IsSpecialSet(character))
                throw new Exception(Exceptions.CHAR_SPECIAL_NOT_FOUND);

            if (!IsValidSkillName(tag1) || !IsValidSkillName(tag2) || !IsValidSkillName(tag3))
                throw new ArgumentException(Exceptions.CHAR_INVALID_TAG_NAMES);

            if (!AreUniqueTags(tag1, tag2, tag3))
                throw new ArgumentException(Exceptions.CHAR_TAGS_NOT_UNIQUE);

            InitializeSkills(character);
            
            SetTagSkill(character, tag1);
            SetTagSkill(character, tag2);
            SetTagSkill(character, tag3);

            await _charService.SaveCharacterAsync(character);
        }

        /// <summary>
        /// Checks if character's skills are set.
        /// </summary>
        public bool AreSkillsSet(Character character)
        {
            if (character == null || character.Skills == null)
                return false;

            var properties = character.Skills.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name.Equals("CharacterId") || prop.Name.Equals("Id"))
                    continue;

                var value = Convert.ToInt32(prop.GetValue(character.Skills));
                if (value == 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Gives character their skill points from leveling up.
        /// </summary>
        public void GiveSkillPoints(Character character)
        {
            if (character == null) throw new ArgumentNullException("character");

            var points = CalculateSkillPoints(character.Special.Intelligence);

            character.SkillPoints += points;
        }

        /// <summary>
        /// Calculate skill points given on level up.
        /// </summary>
        /// <remarks>
        /// Uses the Fallout New Vegas formula. (10 + (INT / 2))
        /// </remarks>
        public int CalculateSkillPoints(int intelligence)
        {
            return DEFAULT_SKILL_POINTS + (intelligence / 2);
        }

        /// <summary>
        /// Puts an amount of points in a specified skill.
        /// </summary>
        public void PutPointsInSkill(Character character, string skill, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!AreSkillsSet(character))
                throw new Exception(Exceptions.CHAR_SKILLS_NOT_SET);

            if (!IsValidSkillName(skill))
                throw new ArgumentException(Exceptions.CHAR_INVALID_SKILL_NAME);

            if (points < 1) return;

            if (points > character.SkillPoints)
                throw new Exception(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            var properties = character.Skills.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name.Equals(skill, StringComparison.InvariantCultureIgnoreCase))
                {
                    var propSkill = Convert.ToInt32(prop.GetValue(character.Skills));

                    if ((propSkill + points) > MAX_SKILL_LEVEL)
                        throw new Exception(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

                    prop.SetValue(character.Skills, (propSkill + points));
                    character.SkillPoints -= points;
                    return;
                }
            }
        }

        /// <summary>
        /// Checks if the tag name matches any of the skill names.
        /// </summary>
        private bool IsValidSkillName(string skill)
        {
            skill = skill.Trim();

            foreach (var name in Globals.SKILL_NAMES)
                if (skill.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if all the tags are unique.
        /// </summary>
        private bool AreUniqueTags(string tag1, string tag2, string tag3)
        {
            if (tag1.Equals(tag2, StringComparison.InvariantCultureIgnoreCase) ||
                tag1.Equals(tag3, StringComparison.InvariantCultureIgnoreCase) ||
                tag2.Equals(tag3, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Sets a character's tag skill.
        /// </summary>
        private void SetTagSkill(Character character, string tag)
        {
            var properties = character.Skills.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase))
                {
                    prop.SetValue(character.Skills, (Convert.ToInt32(prop.GetValue(character.Skills)) + 15));
                }
            }
        }

        /// <summary>
        /// Initializes a character's skills.
        /// </summary>
        private void InitializeSkills(Character character)
        {
            character.Skills = new SkillSheet()
            {
                Barter = CalculateSkill(character.Special.Charisma, character.Special.Luck),
                EnergyWeapons = CalculateSkill(character.Special.Perception, character.Special.Luck),
                Explosives = CalculateSkill(character.Special.Perception, character.Special.Luck),
                Guns = CalculateSkill(character.Special.Agility, character.Special.Luck),
                Lockpick = CalculateSkill(character.Special.Perception, character.Special.Luck),
                Medicine = CalculateSkill(character.Special.Intelligence, character.Special.Luck),
                MeleeWeapons = CalculateSkill(character.Special.Strength, character.Special.Luck),
                Repair = CalculateSkill(character.Special.Intelligence, character.Special.Luck),
                Science = CalculateSkill(character.Special.Intelligence, character.Special.Luck),
                Sneak = CalculateSkill(character.Special.Agility, character.Special.Luck),
                Speech = CalculateSkill(character.Special.Charisma, character.Special.Luck),
                Survival = CalculateSkill(character.Special.Endurance, character.Special.Luck),
                Unarmed = CalculateSkill(character.Special.Endurance, character.Special.Luck)
            };
        }

        /// <summary>
        /// Calculates a skill based on New Vegas formula.
        /// </summary>
        private int CalculateSkill(int stat, int luck)
        {
            return (2 + (2 * stat) + (luck / 2));
        }
    }
}
