using System;
using Discord;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class RollResult : RuntimeResult
    {
        public Embed RollEmbed { get; }
        public string OldMessage { get; }

        public RollResult(Embed embed) : base(null, null)
        {
            RollEmbed = embed;
        }

        public RollResult(string oldMessage) : base(null, null)
        {
            OldMessage = oldMessage;
        }

        public static RollResult DiceRoll(Embed embed) =>
            new RollResult(embed);

        public static RollResult PercentageRoll(string message) =>
            new RollResult(message);
    }
}