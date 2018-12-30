using System;
using Discord.Commands;

namespace FalloutRPG.Constants
{
    public class CharacterResult : RuntimeResult
    {
        public CharacterResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static CharacterResult CharacterNotFound(string mention = "") =>
            new CharacterResult(CommandError.ObjectNotFound, String.Format(Messages.ERR_CHAR_NOT_FOUND, mention));

        public static CharacterResult NpcNotFound(string mention = "") =>
            new CharacterResult(CommandError.ObjectNotFound, String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, mention));
    }
}