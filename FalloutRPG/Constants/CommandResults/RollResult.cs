using System;
using Discord;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class RollResult : RuntimeResult
    {
        public Embed RollEmbed { get; }

        public RollResult(Embed embed) : base(null, null)
        {
            RollEmbed = embed;
        }

        public static RollResult FromSuccess(Embed embed) =>
            new RollResult(embed);
    }
}