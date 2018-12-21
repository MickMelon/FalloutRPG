using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using System.Linq;
using FalloutRPG.Services.Roleplay;

namespace FalloutRPG.Helpers
{
    public class StatisticFlagTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (Enum.TryParse(input, true, out Globals.StatisticFlag result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Statistic Flag not found."));
        }
    }
}
