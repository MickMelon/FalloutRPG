using System;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class StatisticResult : RuntimeResult
    {
        public StatisticResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static StatisticResult NotUsingNewVegasRules() =>
            new StatisticResult(CommandError.Unsuccessful, Messages.ERR_STAT_NOT_USING_NEW_VEGAS_RULES);

        public static StatisticResult UsingOldProgression() =>
            new StatisticResult(CommandError.Unsuccessful, Messages.ERR_STAT_USING_OLD_PROGRESSION);

        public static StatisticResult NotUsingOldProgression() =>
            new StatisticResult(CommandError.Unsuccessful, Messages.ERR_STAT_NOT_USING_OLD_PROGRESSION);

        public static StatisticResult SkillNotHighEnough() =>
            new StatisticResult(CommandError.ObjectNotFound, Messages.ERR_SKILLS_TOO_LOW);

        public static StatisticResult SkillsAlreadyTagged() =>
            new StatisticResult(CommandError.Unsuccessful, Messages.ERR_SKILLS_ALREADY_SET);

        public static StatisticResult SkillsNotSet() =>
            new StatisticResult(CommandError.ObjectNotFound, Messages.ERR_SKILLS_NOT_FOUND);

        public static StatisticResult SpecialNotSet() =>
            new StatisticResult(CommandError.ObjectNotFound, Messages.ERR_SPECIAL_NOT_FOUND);

        public static StatisticResult SpecialAlreadySet() =>
            new StatisticResult(CommandError.ObjectNotFound, Messages.ERR_SPECIAL_ALREADY_SET);

        public static StatisticResult StatisticAlreadyExists() =>
            new StatisticResult(CommandError.Unsuccessful, Messages.ERR_STAT_ALREADY_EXISTS);
    }
}