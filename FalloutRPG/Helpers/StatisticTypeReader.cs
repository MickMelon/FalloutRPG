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
    public class StatisticTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var statService = (StatisticsService)services.GetService(typeof(StatisticsService));
            
            var match = statService.Statistics.FirstOrDefault(x => x.AliasesArray.Contains(input));

            if (match is Statistic s)
                return Task.FromResult(TypeReaderResult.FromSuccess(s));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a Statistic."));
        }
    }
}
