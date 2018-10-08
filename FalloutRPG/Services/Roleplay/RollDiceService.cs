using FalloutRPG.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Services.Roleplay
{
    public class RollDiceService
    {
        private readonly Random _random;

        public RollDiceService(Random random)
        {
            _random = random;
        }

        public int[] RollDice(string diceString)
        {
            if (!diceString.Contains('d') || diceString.Length < 3)
                throw new ArgumentException(Exceptions.ROLL_DICE_INVALID_STRING);

            if (!Int32.TryParse(diceString.Substring(0, diceString.IndexOf('d')), out int dieCount))
                throw new ArgumentException(Exceptions.ROLL_DICE_INVALID_STRING);

            if (dieCount > 20)
                throw new ArgumentException(Exceptions.ROLL_DICE_TOO_MANY);

            diceString = diceString.Substring(diceString.IndexOf('d') + 1);

            int dieSides = 0, bonus = 0;

            if (diceString.Contains('+'))
            {
                if (!Int32.TryParse(diceString.Substring(0, diceString.IndexOf('+')), out dieSides))
                    throw new ArgumentException(Exceptions.ROLL_DICE_INVALID_STRING);
                if (!Int32.TryParse(diceString.Substring(diceString.IndexOf('+') + 1), out bonus))
                    throw new ArgumentException(Exceptions.ROLL_DICE_INVALID_STRING);
            }
            else
            {
                if (!Int32.TryParse(diceString, out dieSides))
                    throw new ArgumentException(Exceptions.ROLL_DICE_INVALID_STRING);
            }

            if (dieSides > 100)
                throw new ArgumentException(Exceptions.ROLL_DICE_TOO_MANY_SIDES);

            int[] dice = new int[dieCount + 2];

            int sum = 0;

            for (int die = 0; die < dieCount; die++)
            {
                int dieResult = _random.Next(1, dieSides + 1);
                dice[die] = dieResult;
                sum += dieResult;
            }

            sum += bonus;
            dice[dice.Length - 2] = bonus;
            dice[dice.Length - 1] = sum;

            return dice;
        }
    }
}
