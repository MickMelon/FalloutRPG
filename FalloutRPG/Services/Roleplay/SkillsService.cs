using FalloutRPG.Constants;
using FalloutRPG.Models;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class SkillsService
    {
        private const int DEFAULT_SKILL_POINTS = 10;

        private const int TAG_ADDITION = 15;

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
        public async Task SetTagSkills(Character character, Globals.SkillType tag1, Globals.SkillType tag2, Globals.SkillType tag3)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!_specService.IsSpecialSet(character))
                throw new Exception(Exceptions.CHAR_SPECIAL_NOT_FOUND);

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

        public SkillSheet CloneSkills(SkillSheet skillSheet)
        {
            var skills = new SkillSheet();

            foreach (var item in typeof(SkillSheet).GetProperties())
                item.SetValue(skills, item.GetValue(skillSheet));

            skills.Id = -1;

            return skills;
        }

        /// <summary>
        /// Returns the value of the specified character's given skill.
        /// </summary>
        /// <returns>Returns 0 if character or skills are null.</returns>
        public int GetSkill(SkillSheet skillSheet, Globals.SkillType skill)
        {
            if (skillSheet == null)
                return 0;

            return (int)typeof(SkillSheet).GetProperty(skill.ToString()).GetValue(skillSheet);
        }

        /// <summary>
        /// Returns the value of the specified character's given skill.
        /// </summary>
        /// <returns>Returns 0 if character or skills are null.</returns>
        public int GetSkill(Character character, Globals.SkillType skill) =>
            GetSkill(character?.Skills, skill);

        /// <summary>
        /// Sets the value of the specified character's given skill.
        /// </summary>
        /// <returns>Returns false if skills are null.</returns>
        public bool SetSkill(SkillSheet skillSheet, Globals.SkillType skill, int newValue)
        {
            if (skillSheet == null)
                return false;

            typeof(SkillSheet).GetProperty(skill.ToString()).SetValue(skillSheet, newValue);
            return true;
        }

        /// <summary>
        /// Sets the value of the specified character's given skill.
        /// </summary>
        /// <returns>Returns false if character or skills are null.</returns>
        public bool SetSkill(Character character, Globals.SkillType skill, int newValue) =>
            SetSkill(character?.Skills, skill, newValue);

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

        public int CalculateSkillPointsForLevel(int intelligence, int level)
        {
            return CalculateSkillPoints(intelligence) * (level - 1);
        }

        /// <summary>
        /// Puts an amount of points in a specified skill.
        /// </summary>
        public void PutPointsInSkill(Character character, Globals.SkillType skill, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!AreSkillsSet(character))
                throw new Exception(Exceptions.CHAR_SKILLS_NOT_SET);

            if (points < 1) return;

            if (points > character.SkillPoints)
                throw new Exception(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            var skillVal = GetSkill(character, skill);

            if ((skillVal + points) > MAX_SKILL_LEVEL)
                throw new Exception(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

            SetSkill(character, skill, skillVal + points);
            character.SkillPoints -= points;
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
        private bool AreUniqueTags(Globals.SkillType tag1, Globals.SkillType tag2, Globals.SkillType tag3)
        {
            if (tag1.Equals(tag2) ||
                tag1.Equals(tag3) ||
                tag2.Equals(tag3))
                return false;

            return true;
        }

        /// <summary>
        /// Sets a character's tag skill.
        /// </summary>
        public void SetTagSkill(Character character, Globals.SkillType tag)
        {
            SetSkill(character, tag, GetSkill(character, tag) + TAG_ADDITION);
        }

        /// <summary>
        /// Initializes a character's skills.
        /// </summary>
        public void InitializeSkills(Character character)
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
