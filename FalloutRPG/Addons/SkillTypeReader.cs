using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;

namespace FalloutRPG.Addons
{
    public class SkillTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (Globals.SKILL_ALIASES.TryGetValue(input, out Globals.SkillType result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a Skill."));
        }
    }
}
