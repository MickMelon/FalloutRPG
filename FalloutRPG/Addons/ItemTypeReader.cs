using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;

namespace FalloutRPG.Addons
{
    public class ItemTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (Enum.TryParse(input, true, out Globals.ItemType result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an item type."));
        }
    }
}
