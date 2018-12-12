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
        private readonly EffectsService _effectsService;
        private readonly SpecialService _specService;
        private readonly SkillsService _skillsService;
        private readonly IConfiguration _config;

        private readonly Random _rand;
        public const int MAX_SUCCESS_ROLL_CAP = 95;

        public RollService(
            EffectsService effectsService,
            SpecialService specService,
            SkillsService skillsService,
            Random rand)
        {
            _effectsService = effectsService;
            _specService = specService;
            _skillsService = skillsService;
            _rand = rand;
        }

        public double GetRollResult(StatisticValue stat)
        {
            double rng = _rand.Next(1, 101);

            double maxSuccessRoll = 0.0;

            if (stat.Statistic is Special)
                maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(stat.Value) - 7);
            else if (stat.Statistic is Skill)
                maxSuccessRoll = Math.Round(10 * Math.Sqrt(stat.Value) - 0.225 * stat.Value - 1);

            // Ensure success is never guaranteed
            if (maxSuccessRoll > MAX_SUCCESS_ROLL_CAP) maxSuccessRoll = MAX_SUCCESS_ROLL_CAP;

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;
            resultPercent = Math.Round(resultPercent * 100.0, 1);

            // TODO: implement luck influence with resultPercent

            return resultPercent;
        }

        public string RollStatistic(Character character, StatisticValue statistic)
        {
            if (statistic.Statistic is Skill skill && statistic.Value < skill.MinimumValue)
                return "too low xd";

            double result = GetRollResult(statistic);

            return $"{GetRollMessage(character.Name, statistic.Statistic.ToString(), result)}";
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
