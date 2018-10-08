using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("roll")]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly RollService _rollService;

        public RollModule(RollService rollService)
        {
            _rollService = rollService;
        }

        [Command]
        public async Task RollDiceAsync(string diceString)
        {
            try
            {
                var dice = _rollService.RollDice(diceString);

                StringBuilder sb = new StringBuilder();
                for (int die = 0; die < dice.Length - 2; die++)
                    sb.Append($"[{dice[die]}] + ");

                sb.Append($"{dice[dice.Length - 2]} ");
                sb.Append($"// Result: {dice[dice.Length-1]}");

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
