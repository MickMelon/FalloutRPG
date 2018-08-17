using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;

namespace FalloutRPG.Addons
{
    public class SpecialTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (Globals.SPECIAL_ALIASES.TryGetValue(input, out Globals.SpecialType result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a Special."));
        }
    }
}
