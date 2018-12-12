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
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an Attribute."));
        }
    }
}
