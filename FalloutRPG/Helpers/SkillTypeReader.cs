using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using System.Linq;
using FalloutRPG.Models;

namespace FalloutRPG.Helpers
{
    public class SkillTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var skillService = (SkillsService)services.GetService(typeof(SkillsService));
            
            var match = skillService.Skills.FirstOrDefault(x => x.AliasesArray.Contains(input));

            if (match is Skill s)
                return Task.FromResult(TypeReaderResult.FromSuccess(s));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a Skill."));
        }
    }
}
