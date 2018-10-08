using Discord;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class RollService
    {
        private readonly CharacterService _charService;
        private readonly SpecialService _specService;
        private readonly SkillsService _skillsService;
        private readonly IConfiguration _config;

        private readonly Random _rand;

        private double LUCK_INFLUENCE;
        private int LUCK_INFLUENCE_SKILL_CUTOFF;
        private bool LUCK_INFLUENCE_ENABLED;

        public RollService(CharacterService charService, SpecialService specService, SkillsService skillsService, IConfiguration config)
        {
            _charService = charService;
            _specService = specService;
            _skillsService = skillsService;
            _config = config;

            LoadLuckInfluenceConfig();

            _rand = new Random();
        }

        /// <summary>
        /// Loads the luck influence configuration from the
        /// configuration file.
        /// </summary>
        private void LoadLuckInfluenceConfig()
        {
            try
            {
                LUCK_INFLUENCE_ENABLED = bool.Parse(_config["roleplay:luck-influenced-rolls"]);

                if (LUCK_INFLUENCE_ENABLED)
                {
                    LUCK_INFLUENCE = double.Parse(_config["roleplay:luck-influence-percentage"]);
                    LUCK_INFLUENCE_SKILL_CUTOFF = int.Parse(_config["roleplay:luck-influence-skill-cutoff"]);

                    if (LUCK_INFLUENCE <= 0 || LUCK_INFLUENCE > 100)
                    {
                        Console.WriteLine("Luck influence settings improperly configured, check Config.json");
                        LUCK_INFLUENCE = 0;
                        
                    }
                    if (LUCK_INFLUENCE_SKILL_CUTOFF <= 0 || LUCK_INFLUENCE_SKILL_CUTOFF > SkillsService.MAX_SKILL_LEVEL)
                    {
                        Console.WriteLine("Luck influence skill cutoff setting improperly configured, check Config.json");
                        LUCK_INFLUENCE_SKILL_CUTOFF = 0;
                    }
                }
                else
                {
                    LUCK_INFLUENCE = 0;
                    LUCK_INFLUENCE_SKILL_CUTOFF = 0;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Luck influence settings improperly configured, check Config.json");
                LUCK_INFLUENCE = 0;
                LUCK_INFLUENCE_SKILL_CUTOFF = 0;
            }
        }

        public async Task<string> GetSpRollAsync(IUser user, Globals.SpecialType specialToRoll)
        {
            var character = await _charService.GetCharacterAsync(user.Id);

            if (character == null)
            {
                return String.Format(Messages.ERR_CHAR_NOT_FOUND, user.Mention);
            }

            if (!_specService.IsSpecialSet(character))
            {
                return String.Format(Messages.ERR_SPECIAL_NOT_FOUND, user.Mention);
            }

            return GetRollMessage(character.Name, specialToRoll.ToString(), GetRollResult(specialToRoll, character));
        }

        public async Task<string> GetSkillRollAsync(IUser user, Globals.SkillType skillToRoll)
        {
            var character = await _charService.GetCharacterAsync(user.Id);

            if (character == null)
            {
                return String.Format(Messages.ERR_CHAR_NOT_FOUND, user.Mention);
            }

            if (!_specService.IsSpecialSet(character))
            {
                return String.Format(Messages.ERR_SPECIAL_NOT_FOUND, user.Mention);
            }

            if (!_skillsService.AreSkillsSet(character))
            {
                return String.Format(Messages.ERR_SKILLS_NOT_FOUND, user.Mention);
            }

            return GetRollMessage(character.Name, skillToRoll.ToString(), GetRollResult(skillToRoll, character));
        }

        public double GetRollResult(Globals.SkillType attribute, Character character)
        {
            var charSkills = character.Skills;
            var charSpecial = character.Special;

            // match given attribute string to property in character
            int attributeValue = 0;
            int rng = 0;

            attributeValue = (int)typeof(SkillSheet).GetProperty(attribute.ToString()).GetValue(charSkills);
            rng = _rand.Next(1, 101);

            // affects odds by the percentage of LUCK_INFLUENCE for each point of luck above or below 5.
            // i.e. if you have 6 luck, and LUCK_INFLUENCE == 5, then now your odds are multiplied by 0.95,
            // which is a good thing.
            int luckDifference = charSpecial.Luck - 5;
            double luckMultiplier = 1.0 - (luckDifference * (LUCK_INFLUENCE / 100.0));

            double finalResult;

            // NEW FORMULA:
            // 10 * Sqrt(x) - (0.225x) - 1 (cap at 200 skill)
            // luck will now not work with skills over LUCK_INFLUENCE_SKILL_CUTOFF
            double maxSuccessRoll = Math.Round(10 * Math.Sqrt(attributeValue) - (0.225 * attributeValue) - 1);

            // Ensures that Luck influence never guarantees success
            if (LUCK_INFLUENCE_ENABLED && attributeValue < LUCK_INFLUENCE_SKILL_CUTOFF && 95 * luckMultiplier > maxSuccessRoll)
                finalResult = rng * luckMultiplier;
            else
                finalResult = rng;

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (maxSuccessRoll - finalResult) / maxSuccessRoll;
            // make it pretty for chat
            resultPercent = Math.Round(resultPercent * 100.0, 1);

            return resultPercent;
        }

        public double GetRollResult(Globals.SpecialType attribute, Character character)
        {
            var charSkills = character.Skills;
            var charSpecial = character.Special;

            // match given attribute string to property in character
            int attributeValue = 0;
            int rng = 0;

            attributeValue = (int)typeof(Special).GetProperty(attribute.ToString()).GetValue(charSpecial);
            rng = _rand.Next(1, 101);

            // affects odds by the percentage of LUCK_INFLUENCE for each point of luck above or below 5.
            // i.e. if you have 6 luck, and LUCK_INFLUENCE == 5, then now your odds are multiplied by 0.95,
            // which is a good thing.
            int luckDifference = charSpecial.Luck - 5;
            double luckMultiplier = 1.0 - (luckDifference * (LUCK_INFLUENCE / 100.0));

            double finalResult;

            double maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(attributeValue) - 7);

            // prevents luck from guaranteeing success
            if (LUCK_INFLUENCE_ENABLED && 95 * luckMultiplier > maxSuccessRoll)
                finalResult = rng * luckMultiplier;
            else
                finalResult = rng;

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (maxSuccessRoll - finalResult) / maxSuccessRoll;
            // make it pretty for chat
            resultPercent = Math.Round(resultPercent * 100.0, 1);

            return resultPercent;
        }

        private string GetRollMessage(string charName, string roll, double percent)
        {
            var result = new StringBuilder();

            if (percent >= 0)
            {
                if (percent >= 125)
                    result.Append($"**CRITICAL {roll.ToUpper()} SUCCESS!!!**");
                else if (percent >= 80)
                    result.Append($"__GREAT {roll.ToUpper()} SUCCESS__");
                else if (percent >= 50)
                    result.Append($"*Very good {roll} success*");
                else if (percent >= 25)
                    result.Append($"*Good {roll} success*");
                else if (percent >= 10)
                    result.Append($"*Above average {roll} success*");
                else
                    result.Append($"__***CLOSE CALL! {roll} success***__");

                result.Append($" for {charName}: did **{percent}%** better than needed!");
            }
            else
            {
                if (percent <= -125)
                    result.Append($"**CRITICAL {roll.ToUpper()} FAILURE!!!**");
                else if (percent <= -80)
                    result.Append($"__TERRIBLE {roll.ToUpper()} FAILURE__");
                else if (percent <= -50)
                    result.Append($"*Pretty bad {roll} failure*");
                else if (percent <= -25)
                    result.Append($"*Bad {roll} failure*");
                else if (percent <= -10)
                    result.Append($"*Above average {roll} failure*");
                else
                    result.Append($"__***Heartbreaking {roll} failure***__");

                result.Append($" for {charName}: did **{percent*-1}%** worse than needed!");
            }

            return result.ToString();
        }
    }
}
