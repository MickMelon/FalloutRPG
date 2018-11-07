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

        public int[] RollDice(int dieCount, int sides)
        {
            if (dieCount > 20)
                throw new ArgumentException(Exceptions.ROLL_DICE_TOO_MANY);

            if (sides > 100)
                throw new ArgumentException(Exceptions.ROLL_DICE_TOO_MANY_SIDES);

            int[] dice = new int[dieCount];

            int sum = 0;

            for (int die = 0; die < dieCount; die++)
            {
                int dieResult = _random.Next(1, sides + 1);
                dice[die] = dieResult;
                sum += dieResult;
            }

            return dice;
        }
    }
}
