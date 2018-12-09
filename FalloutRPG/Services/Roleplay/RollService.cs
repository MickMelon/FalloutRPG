﻿using Discord;
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
        private readonly EffectsService _effectsService;
        private readonly SpecialService _specService;
        private readonly SkillsService _skillsService;
        private readonly IConfiguration _config;

        private readonly Random _rand;
        public const int MAX_SUCCESS_ROLL_CAP = 95;

        private double LUCK_INFLUENCE;
        private int LUCK_INFLUENCE_SKILL_CUTOFF;
        private int LUCK_INFLUENCE_SPECIAL_CUTOFF;
        private bool LUCK_INFLUENCE_ENABLED;

        public RollService(
            EffectsService effectsService,
            SpecialService specService,
            SkillsService skillsService,
            IConfiguration config,
            Random rand)
        {
            _effectsService = effectsService;
            _specService = specService;
            _skillsService = skillsService;
            _config = config;
            _rand = rand;

            LoadLuckInfluenceConfig();
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
                    LUCK_INFLUENCE_SPECIAL_CUTOFF = int.Parse(_config["roleplay:luck-influence-special-cutoff"]);

                    if (LUCK_INFLUENCE <= 0 || LUCK_INFLUENCE > 100)
                    {
                        Console.WriteLine("Luck influence settings improperly configured, check Config.json");
                        LUCK_INFLUENCE = 0;
                    }

                    if (LUCK_INFLUENCE_SKILL_CUTOFF <= 0 || LUCK_INFLUENCE_SKILL_CUTOFF > SkillsService.MAX_SKILL_LEVEL ||
                        LUCK_INFLUENCE_SPECIAL_CUTOFF <=  0 || LUCK_INFLUENCE_SPECIAL_CUTOFF > SpecialService.MAX_SPECIAL)
                    {
                        Console.WriteLine("Luck influence skill or special cutoff setting improperly configured, check Config.json");
                        LUCK_INFLUENCE_SKILL_CUTOFF = 0;
                        LUCK_INFLUENCE_SPECIAL_CUTOFF = 0;
                    }
                }
                else
                {
                    LUCK_INFLUENCE = 0;
                    LUCK_INFLUENCE_SKILL_CUTOFF = 0;
                    LUCK_INFLUENCE_SPECIAL_CUTOFF = 0;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Luck influence settings improperly configured, check Config.json");
                LUCK_INFLUENCE = 0;
                LUCK_INFLUENCE_SKILL_CUTOFF = 0;
            }
        }

        public double GetRollResult(int attribute, int luck, bool isSpecial)
        {
            double rng = _rand.Next(1, 101);

            double maxSuccessRoll;

            if (isSpecial)
                maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(attribute) - 7);
            else
                maxSuccessRoll = Math.Round(10 * Math.Sqrt(attribute) - 0.225 * attribute - 1);

            double luckMultiplier = 1.0 - (luck - 5 * (LUCK_INFLUENCE / 100.0));

            // Ensures that Luck influence never guarantees success
            if (LUCK_INFLUENCE_ENABLED && attribute < LUCK_INFLUENCE_SKILL_CUTOFF && MAX_SUCCESS_ROLL_CAP * luckMultiplier > maxSuccessRoll)
                rng *= luckMultiplier;

            // Ensure success is never guaranteed
            if (maxSuccessRoll > MAX_SUCCESS_ROLL_CAP) maxSuccessRoll = MAX_SUCCESS_ROLL_CAP;

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;
            resultPercent = Math.Round(resultPercent * 100.0, 1);

            return resultPercent;
        }

        public string RollAttribute(Character character, Globals.SkillType skill, bool useEffects) =>
            RollAttribute(character, (Enum)skill, useEffects);

        public string RollAttribute(Character character, Globals.SpecialType special, bool useEffects) =>
            RollAttribute(character, (Enum)special, useEffects);

        private string RollAttribute(Character character, Enum attribute, bool useEffects)
        {
            double result = 0.0;
            Special special = character.Special;
            SkillSheet skills = character.Skills;

            if (useEffects)
            {
                special = _effectsService.GetEffectiveSpecial(character);
                skills = _effectsService.GetEffectiveSkills(character);
            }

            if (attribute is Globals.SpecialType)
            {
                result = GetRollResult(_specService.GetSpecial(special, (Globals.SpecialType)attribute), special.Luck, true);
            }
            else if (attribute is Globals.SkillType)
            {
                result = GetRollResult(_skillsService.GetSkill(skills, (Globals.SkillType)attribute), special.Luck, false);
            }

            return $"{GetRollMessage(character.Name, attribute.ToString(), result)}";
        }

        public string GetRollMessage(string charName, string roll, double percent)
        {
            var result = new StringBuilder();

            if (percent >= 0)
            {
                if (percent >= 95)
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
