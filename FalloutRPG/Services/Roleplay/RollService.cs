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

        private int LUCK_INFLUENCE;
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
                    LUCK_INFLUENCE = int.Parse(_config["roleplay:luck-influence-percentage"]);
                    if (LUCK_INFLUENCE <= 0 || LUCK_INFLUENCE > int.MaxValue)
                    {
                        Console.WriteLine("Luck influence settings improperly configured, check Config.json");
                        LUCK_INFLUENCE = 0;
                    }
                }
                else
                    LUCK_INFLUENCE = 0;
            }
            catch (Exception)
            {
                Console.WriteLine("Luck influence settings improperly configured, check Config.json");
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

            attributeValue = _skillsService.GetSkill(character, attribute);
            rng = _rand.Next(1, 101);

            // affects odds by the percentage of LUCK_INFLUENCE for each point of luck above or below 5.
            // i.e. if you have 6 luck, and LUCK_INFLUENCE == 5, then now your odds are multiplied by 0.95,
            // which is a good thing.
            int luckDifference = charSpecial.Luck - 5;
            double luckMultiplier = 1.0 - (luckDifference * (LUCK_INFLUENCE / 100.0));

            double finalResult;

            if (LUCK_INFLUENCE_ENABLED)
                finalResult = rng * luckMultiplier;
            else
                finalResult = rng;

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (attributeValue - finalResult) / attributeValue;
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

            attributeValue = _specService.GetSpecial(character, attribute);
            rng = _rand.Next(1, 11);

            // affects odds by the percentage of LUCK_INFLUENCE for each point of luck above or below 5.
            // i.e. if you have 6 luck, and LUCK_INFLUENCE == 5, then now your odds are multiplied by 0.95,
            // which is a good thing.
            int luckDifference = charSpecial.Luck - 5;
            double luckMultiplier = 1.0 - (luckDifference * (LUCK_INFLUENCE / 100.0));

            double finalResult;

            if (LUCK_INFLUENCE_ENABLED)
                finalResult = rng * luckMultiplier;
            else
                finalResult = rng;

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (attributeValue - finalResult) / attributeValue;
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
