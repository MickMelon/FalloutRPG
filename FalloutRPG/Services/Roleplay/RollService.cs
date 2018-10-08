using FalloutRPG.Constants;
using FalloutRPG.Models;
using System;

namespace FalloutRPG.Services.Roleplay
{
    public class RollService
    {
        private readonly CharacterService _charService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specService;

        private readonly Random _rand;

        public RollService(CharacterService charService,
            SkillsService skillsService,
            SpecialService specService,
            Random rand)
        {
            _charService = charService;
            _specService = specService;
            _skillsService = skillsService;

            _rand = rand;
        }

        private double GetRollResult(int attribute)
        {
            int rng = _rand.Next(1, 101);

            double maxSuccessRoll = Math.Round(10 * Math.Sqrt(attribute) - (0.225 * attribute) - 1);

            // compares your roll with your skills, and how much better you did than the bare minimum
            double resultPercent = (maxSuccessRoll - rng) / maxSuccessRoll;
            resultPercent = Math.Round(resultPercent * 100.0, 1);

            return resultPercent;
        }

        public double GetRollResult(Character character, Globals.SpecialType attribute) =>
            GetRollResult(_specService.GetSpecial(character, attribute));

        public double GetRollResult(Character character, Globals.SkillType skill) =>
            GetRollResult(_skillsService.GetSkill(character, skill));
    }
}
