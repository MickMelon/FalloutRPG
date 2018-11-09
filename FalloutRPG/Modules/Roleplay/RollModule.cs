using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        [Group("roll")]
        public class RollDiceModule : ModuleBase<SocketCommandContext>
        {
            private readonly RollDiceService _diceService;

            public RollDiceModule(RollDiceService rollService)
            {
                _diceService = rollService;
            }

            [Command]
            public async Task RollDiceAsync(int dieCount, int sides, int bonus = 0)
            {
                try
                {
                    var dice = _diceService.RollDice(dieCount, sides);

                    StringBuilder sb = new StringBuilder();
                    for (int die = 0; die < dice.Length; die++)
                        sb.Append($"[{dice[die]}] + ");

                    sb.Append($"{bonus} = {dice.Sum() + bonus}");

                    await ReplyAsync(String.Format(Messages.ROLL_DICE, sb.ToString(), Context.User.Mention));
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({Context.User.Mention})");
                    return;
                }
            }
        }

        [Group("rollvs")]
        public class RollVsModule : ModuleBase<SocketCommandContext>
        {
            private readonly CharacterService _characterService;
            private readonly RollVsService _rollVsService;
            private readonly NpcService _npcService;

            public RollVsModule(CharacterService characterService, RollVsService rollVsService, NpcService npcService)
            {
                _characterService = characterService;
                _rollVsService = rollVsService;
                _npcService = npcService;
            }

            [Command]
            public async Task RollVsAsync(string charName, Globals.SkillType skill)
            {
                await Task.Delay(0);
            }
        }
    }
}
