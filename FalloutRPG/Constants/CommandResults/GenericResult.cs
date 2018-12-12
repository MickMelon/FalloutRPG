using System;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class GenericResult : RuntimeResult
    {
        public GenericResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static GenericResult FromError(string reason) =>
            new GenericResult(CommandError.Unsuccessful, reason);

        public static GenericResult FromSuccess(string reason = null) =>
            new GenericResult(null, reason);
    }
}