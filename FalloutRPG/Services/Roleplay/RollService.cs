using Discord;
using FalloutRPG.Constants;
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

        public int[] GetRollResult(Character character, Statistic stat, int extraDie = 0)
        {
            int numOfDie = _statService.GetStatistic(character, stat) + extraDie;

            if (stat is Skill skill)
            {
                var specValue = _statService.GetStatistic(character, skill.Special);

                // TODO: maybe should make GetStatistic return a Nullable?
                if (specValue != -1)
                    numOfDie += specValue;
            }
                

            int[] die = new int[numOfDie];

            for (int dice = 0; dice < die.Length; dice++)
                die[dice] = _rand.Next(1, DICE_SIDES + 1);

            return die;
        }

        public string RollStatistic(Character character, Statistic stat)
        {
            if (stat is Skill skill && _statService.GetStatistic(character, stat) < skill.MinimumValue)
                return Messages.ERR_SKILLS_TOO_LOW;

            var result = GetRollResult(character, stat);

            return $"{GetRollMessage(character.Name, stat.ToString(), result)}";
        }

        public string GetRollMessage(string charName, string roll, int[] result)
        {
            var message = new StringBuilder($"{charName} rolls {roll}:\n\n");

            foreach (var dice in result)
                message.Append($"[{dice}] ");

            int successes = result.Count(x => x >= SUCCESS_ROLL);

            if (successes > 7)
                message.Append($"**AMAZING SUCCESS: {successes}!!**");
            else
                message.Append($"*Successes: {successes}*");

            return result.ToString();
        }
    }
}
