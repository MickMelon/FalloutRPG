using Discord;
using FalloutRPG.Models.Configuration;
using FalloutRPG.Services.Roleplay;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Casino
{
    public class GamblingService
    {
        public readonly Dictionary<IUser, long> UserBalances;

        private readonly CharacterService _charService;
        private readonly GamblingOptions _gamblingOptions;

        public GamblingService(CharacterService charService, GamblingOptions gamblingOptions)
        {
            UserBalances = new Dictionary<IUser, long>();
            
            _charService = charService;
            _gamblingOptions = gamblingOptions;
        }

        public async Task UpdateBalances()
        {
            foreach (var user in UserBalances)
            {
                var character = await _charService.GetCharacterAsync(user.Key.Id);

                if (character == null)
                    continue;

                if (character.Money != user.Value)
                {
                    character.Money = user.Value;
                    await _charService.SaveCharacterAsync(character);
                }
            }
        }

        public bool IsGamblingEnabledChannel(ulong channelId)
        {
            if (_gamblingOptions.EnabledChannels.Contains(channelId))
                return true;
            return false;
        }

        /// <summary>
        /// This will create an entry in UserBalances with the specified user, and their balance
        /// </summary>
        /// <param name="user">The user to add their balance to UserBalances</param>
        /// <returns>A boolean stating whether the user's balance was added or not.</returns>
        public async Task<AddUserBalanceResult> AddUserBalanceAsync(IUser user)
        {
            var character = await _charService.GetCharacterAsync(user.Id);

            if (character == null)
                return AddUserBalanceResult.NullCharacter;

            if (UserBalances.ContainsKey(user))
                return AddUserBalanceResult.AlreadyInDictionary;

            UserBalances.Add(user, character.Money);
            return AddUserBalanceResult.Success;
        }

        public enum AddUserBalanceResult
        {
            Success,
            AlreadyInDictionary,
            NullCharacter,
            UnknownError
        }
    }
}
