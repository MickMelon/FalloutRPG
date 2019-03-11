using System;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class CharacterResult : RuntimeResult
    {
        public CharacterResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static CharacterResult CharacterNotFound() =>
            new CharacterResult(CommandError.ObjectNotFound, Messages.ERR_CHAR_NOT_FOUND);

        public static CharacterResult NpcNotFound() =>
            new CharacterResult(CommandError.ObjectNotFound, Messages.ERR_NPC_CHAR_NOT_FOUND);
    }
}