using Discord.Commands;
using FalloutRPG.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("roll")]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly Random _random;

        public RollModule(Random random)
        {
            _random = random;
        }

        [Command]
        public async Task RollDiceAsync(string diceString)
        {
            if (!diceString.Contains('d') || diceString.Length < 3)
            {
                await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_INVALID_STRING, Context.User.Mention));
                return;
            }
            if (!Int32.TryParse(diceString.Substring(0, diceString.IndexOf('d')), out int dieCount))
            {
                await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_INVALID_STRING, Context.User.Mention));
                return;
            }
            Console.WriteLine(dieCount);
            if (dieCount > 20)
            {
                await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_TOO_MANY, Context.User.Mention));
                return;
            }

            diceString = diceString.Substring(diceString.IndexOf('d') + 1);

            int dieSides = 0, bonus = 0;

            if (diceString.Contains('+'))
            {
                if (!Int32.TryParse(diceString.Substring(0, diceString.IndexOf('+')), out dieSides))
                {
                    await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_INVALID_STRING, Context.User.Mention));
                    return;
                }
                if (!Int32.TryParse(diceString.Substring(diceString.IndexOf('+')+1), out bonus))
                {
                    await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_INVALID_STRING, Context.User.Mention));
                    return;
                }
            }
            else
            {
                if (!Int32.TryParse(diceString, out dieSides))
                {
                    await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_INVALID_STRING, Context.User.Mention));
                    return;
                }
            }
            Console.WriteLine(dieSides);
            if (dieSides > 100)
            {
                await ReplyAsync(String.Format(Messages.ERR_ROLL_DICE_TOO_MANY_SIDES, Context.User.Mention));
                return;
            }
            StringBuilder sb = new StringBuilder();
            int sum = 0;
            for (int die = 0; die < dieCount; die++)
            {
                int dieResult = _random.Next(1, dieSides + 1);
                sum += dieResult;
                sb.Append($"[{dieResult}] + ");
            }
            sum += bonus;
            sb.Append($"{bonus} || Result: {sum}");
            await ReplyAsync(String.Format(Messages.ROLL_DICE, sb.ToString(), Context.User.Mention));
        }
    }
}
