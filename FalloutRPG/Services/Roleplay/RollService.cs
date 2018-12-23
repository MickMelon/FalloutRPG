using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
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
        private readonly SpecialService _specService;
        private readonly SkillsService _skillsService;
        private readonly StatisticsService _statService;

        private readonly Random _rand;

        private const int DICE_SIDES = 8;
        private const int SUCCESS_ROLL = 8;

        public RollService(
            EffectsService effectsService,
            SpecialService specService,
            SkillsService skillsService,
            StatisticsService statService,
            Random rand)
        {
            _effectsService = effectsService;
            _specService = specService;
            _skillsService = skillsService;
            _statService = statService;
            _rand = rand;
        }

        public int[] GetRollResult(IList<StatisticValue> stats, Statistic stat)
        {
            var statValue = _statService.GetStatistic(stats, stat);

            int numOfDie = statValue;

            if (stat is Skill skill)
            {
                var specValue = _statService.GetStatistic(stats, skill.Special);

                numOfDie += specValue;
            }

            int[] die = new int[numOfDie];

            for (int dice = 0; dice < die.Length; dice++)
                die[dice] = _rand.Next(1, DICE_SIDES + 1);

            return die;
        }

        public RuntimeResult RollStatistic(Character character, Statistic stat, bool useEffects = false)
        {
            var stats = character.Statistics;

            if (useEffects)
                stats = _effectsService.GetEffectiveStatistics(character);

            if (stat is Skill skill && _statService.GetStatistic(stats, stat) < skill.MinimumValue)
                return StatisticResult.SkillNotHighEnough();

            var result = GetRollResult(stats, stat);
            var successes = result.Count(x => x >= SUCCESS_ROLL);

            var message = $"*{character.Name}* rolls `{stat.Name}`!\n" + GetRollMessage(character.Name, stat.Name, result);

            Color color = new Color(200, 45, 0);

            if (successes > 7) color = new Color(210, 170, 0);
            else if (successes > 0) color = new Color(60, 210, 0);

            if (useEffects)
                return RollResult.FromSuccess(EmbedHelper.BuildBasicEmbed($"{Messages.MUSCLE_EMOJI}{stat.Name} Buffed Roll", message, color));

            return RollResult.FromSuccess(EmbedHelper.BuildBasicEmbed($"{stat.Name} Roll", message, color));
        }

        public RuntimeResult RollVsStatistic(Character character, Character character2, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var stats1 = character.Statistics;
            var stats2 = character.Statistics;

            if (useEffects)
            {
                stats1 = _effectsService.GetEffectiveStatistics(character);
                stats2 = _effectsService.GetEffectiveStatistics(character2);
            }

            if ((stat1 is Skill skill1 && _statService.GetStatistic(stats1, stat1) < skill1.MinimumValue) ||
                (stat2 is Skill skill2 && _statService.GetStatistic(stats2, stat2) < skill2.MinimumValue))
                return StatisticResult.SkillNotHighEnough();

            var result = GetRollResult(stats1, stat1);
            var result2 = GetRollResult(stats2, stat2);

            var message = $"*{character.Name}* rolls `{stat1.Name}` against *{character2.Name}'s* `{stat2.Name}`!\n\n" +
                $"__{character.Name}__: {GetRollMessage(character.Name, stat1.Name, result)}\n\n" +
                $"__{character2.Name}__: {GetRollMessage(character2.Name, stat2.Name, result2)}";

            if (useEffects)
                return RollResult.FromSuccess(EmbedHelper.BuildBasicEmbed($"{Messages.MUSCLE_EMOJI}{stat1.Name} Vs. {stat2.Name} Buffed Roll", message));

            return RollResult.FromSuccess(EmbedHelper.BuildBasicEmbed($"{stat1.Name} Vs. {stat2.Name} Roll", message));
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
    }
}
