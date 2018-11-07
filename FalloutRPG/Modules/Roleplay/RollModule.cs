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
    [Group("roll")]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly RollDiceService _rollService;

        public RollModule(RollDiceService rollService)
        {
            _rollService = rollService;
        }

        [Command]
        public async Task RollDiceAsync(int dieCount, int sides, int bonus = 0)
        {
            try
            {
                var dice = _rollService.RollDice(dieCount, sides);

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
}
