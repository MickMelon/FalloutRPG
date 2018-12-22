using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using System.Linq;
using FalloutRPG.Models;

namespace FalloutRPG.Helpers
{
    public class SpecialTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var specService = (SpecialService)services.GetService(typeof(SpecialService));
            
            var match = specService.Specials.FirstOrDefault(x => x.AliasesArray.Contains(input, StringComparer.OrdinalIgnoreCase));

            if (match is Special s)
                return Task.FromResult(TypeReaderResult.FromSuccess(s));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Special not found."));
        }
    }
}
