using Discord.Commands;
using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using System.Linq;

namespace FalloutRPG.Addons
{
    public class StatisticTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var statRepo = (IRepository<Statistic>)services.GetService(typeof(IRepository<Statistic>));
            
            var match = statRepo.Query.FirstOrDefault(x => x.AliasesArray.Contains(input));

            if (match is Statistic s)
                return Task.FromResult(TypeReaderResult.FromSuccess(s));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a Statistic."));
        }
    }
}
