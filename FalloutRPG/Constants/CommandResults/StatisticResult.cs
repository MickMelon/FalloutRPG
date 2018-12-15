using System;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class StatisticResult : RuntimeResult
    {
        public StatisticResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static StatisticResult SkillNotHighEnough(string mention = "") =>
            new StatisticResult(CommandError.ObjectNotFound, String.Format(Messages.ERR_SKILLS_TOO_LOW, mention));

        public static StatisticResult SkillsAlreadyTagged(string mention = "") =>
            new StatisticResult(CommandError.Unsuccessful, String.Format(Messages.ERR_SKILLS_ALREADY_SET, mention));

        public static StatisticResult SkillsNotSet(string mention = "") =>
            new StatisticResult(CommandError.ObjectNotFound, String.Format(Messages.ERR_SKILLS_NOT_FOUND, mention));

        public static StatisticResult SpecialNotSet(string mention = "") =>
            new StatisticResult(CommandError.ObjectNotFound, String.Format(Messages.ERR_SPECIAL_NOT_FOUND, mention));

        public static StatisticResult StatisticAlreadyExists(string mention = "") =>
            new StatisticResult(CommandError.Unsuccessful, String.Format(Messages.ERR_STAT_ALREADY_EXISTS, mention));
    }
}