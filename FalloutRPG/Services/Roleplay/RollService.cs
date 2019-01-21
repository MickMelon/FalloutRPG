using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Models.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class RollService
    {
        private readonly EffectsService _effectsService;
        private readonly RoleplayOptions _roleplayOptions;
        private readonly SpecialService _specService;
        private readonly SkillsService _skillsService;
        private readonly StatisticsService _statService;

        private readonly Random _rand;

        private const int DICE_SIDES = 8;
        private const int SUCCESS_ROLL = 8;

        public RollService(
            EffectsService effectsService,
            RoleplayOptions roleplayOptions,
            SpecialService specService,
            SkillsService skillsService,
            StatisticsService statService,
            Random rand)
        {
            _effectsService = effectsService;
            _roleplayOptions = roleplayOptions;
            _specService = specService;
            _skillsService = skillsService;
            _statService = statService;

            _rand = rand;
        }

        private int GetNumberOfDice(IList<StatisticValue> stats, Statistic stat)
        {
            var statValue = _statService.GetStatistic(stats, stat);

            int numOfDie = statValue;

            if (stat is Skill skill)
            {
                var specValue = _statService.GetStatistic(stats, skill.Special);

                numOfDie += specValue;
            }

            return numOfDie;
        }

        public int[] GetRollResult(IList<StatisticValue> stats, Statistic stat)
        {
            int numOfDie = GetNumberOfDice(stats, stat);

            int[] die = new int[numOfDie];

            for (int dice = 0; dice < die.Length; dice++)
                die[dice] = _rand.Next(1, DICE_SIDES + 1);

            return die;
        }

        public double GetOldRollResult(IList<StatisticValue> stats, Statistic stat)
        {
            double rng = _rand.Next(1, 101);

            double maxSuccessRoll = CalculateProbability(DICE_SIDES, GetNumberOfDice(stats, stat)) * 100;

            if (_roleplayOptions.UseOldProbability)
            {
                var statValue = _statService.GetStatistic(stats, stat);

                if (stat is Special)
                    maxSuccessRoll = Math.Round(32.2 * Math.Sqrt(statValue) - 7);
                else
                    maxSuccessRoll = Math.Round(10 * Math.Sqrt(statValue) - 0.225 * statValue - 1);
            }

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;
            resultPercent = Math.Round(resultPercent * 100.0, 1);

            return resultPercent;
        }

        public RuntimeResult RollStatistic(Character character, Statistic stat, bool useEffects = false)
        {
            var stats = character.Statistics;

            if (useEffects)
                stats = _effectsService.GetEffectiveStatistics(character);

            if (stat is Skill skill && _statService.GetStatistic(stats, stat) < skill.MinimumValue)
                return StatisticResult.SkillNotHighEnough();

            string message = "";
            Color color = new Color(200, 45, 0);

            if (_roleplayOptions.UsePercentage)
            {
                var result = GetOldRollResult(stats, stat);

                message = GetOldRollMessage(character.Name, stat.Name, result);

                if (result >= 95) color = new Color(210, 170, 0);
                else if (result >= 0) color = new Color(60, 210, 0);

                if (useEffects)
                    message = Messages.MUSCLE_EMOJI + message;

                return RollResult.PercentageRoll(message);
            }
            else
            {
                var result = GetRollResult(stats, stat);
                var successes = result.Count(x => x >= SUCCESS_ROLL);

                message = $"*{character.Name}* rolls `{stat.Name}`!\n" + GetRollMessage(character.Name, stat.Name, result);

                if (successes > 7) color = new Color(210, 170, 0);
                else if (successes > 0) color = new Color(60, 210, 0);

                if (useEffects)
                    return RollResult.DiceRoll(EmbedHelper.BuildBasicEmbed($"{Messages.MUSCLE_EMOJI}{stat.Name} Buffed Roll", message, color));

                return RollResult.DiceRoll(EmbedHelper.BuildBasicEmbed($"{stat.Name} Roll", message, color));
            }   
        }

        public RuntimeResult RollVsStatistic(Character character1, Character character2, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var stats1 = character1.Statistics;
            var stats2 = character2.Statistics;

            if (useEffects)
            {
                stats1 = _effectsService.GetEffectiveStatistics(character1);
                stats2 = _effectsService.GetEffectiveStatistics(character2);
            }

            if ((stat1 is Skill skill1 && _statService.GetStatistic(stats1, stat1) < skill1.MinimumValue) ||
                (stat2 is Skill skill2 && _statService.GetStatistic(stats2, stat2) < skill2.MinimumValue))
                return StatisticResult.SkillNotHighEnough();

            string message = "";

            if (_roleplayOptions.UsePercentage)
            {
                var result1 = GetOldRollResult(stats1, stat1);
                var result2 = GetOldRollResult(stats2, stat2);

                message = $"*{character1.Name}* rolls `{stat1.Name}` against *{character2.Name}'s* `{stat2.Name}`!\n\n" +
                    $"{GetOldRollMessage(character1.Name, stat1.Name, result1)}\n" +
                    $"{GetOldRollMessage(character2.Name, stat2.Name, result2)}\n\n";

                if (result1 < 0 && result2 < 0)
                {
                    message += "Nobody wins!";
                }
                else
                {
                    if (result1 > result2) message += $"{character1.Name} wins!";
                    else message += $"{character2.Name} wins!";
                }

                if (useEffects)
                    message = Messages.MUSCLE_EMOJI + message;

                return RollResult.PercentageRoll(message);
            }
            else
            {
                var result1 = GetRollResult(stats1, stat1);
                var result2 = GetRollResult(stats2, stat2);

                message = $"*{character1.Name}* rolls `{stat1.Name}` against *{character2.Name}'s* `{stat2.Name}`!\n\n" +
                    $"__{character1.Name}__: {GetRollMessage(character1.Name, stat1.Name, result1)}\n\n" +
                    $"__{character2.Name}__: {GetRollMessage(character2.Name, stat2.Name, result2)}";

                if (useEffects)
                    return RollResult.DiceRoll(EmbedHelper.BuildBasicEmbed($"{Messages.MUSCLE_EMOJI}{stat1.Name} Vs. {stat2.Name} Buffed Roll", message));

                return RollResult.DiceRoll(EmbedHelper.BuildBasicEmbed($"{stat1.Name} Vs. {stat2.Name} Roll", message));
            }
        }

        public string GetRollMessage(string charName, string statName, int[] result)
        {
            var message = new StringBuilder("**[");

            // Prevents trailing comma ( [ 1, 2, 3, ] )
            for (int dice = 0; dice < result.Length - 1; dice++)
                message.Append($"{result[dice]}, ");

            message.Append($"{result[result.Length - 1]}]**");

            int successes = result.Count(x => x >= SUCCESS_ROLL);

            if (successes > 7)
            {
                message.Append($"\n**AMAZING SUCCESS: {successes}!!**");
            }
            else
            {
                message.Append($"\n*Successes: {successes}*");
            }

            return message.ToString();
        }

        public string GetOldRollMessage(string charName, string roll, double percent)
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

                result.Append($" for {charName}: did **{percent * -1}%** worse than needed!");
            }

            return result.ToString();
        }

        static double CalculateProbability(int diceSides, int diceAmount)
        {
            double result = (diceSides - 1.0) / diceSides;

            for (int i = 1; i < diceAmount; i++)
                result *= (diceSides - 1.0) / diceSides;

            return 1 - result;
        }
    }
}
