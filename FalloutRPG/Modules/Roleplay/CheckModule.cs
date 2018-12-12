using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("check")]
    public class CheckModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;

        public CheckModule(CharacterService charService, SkillsService skillsService, SpecialService specialService)
        {
            _charService = charService;
            _skillsService = skillsService;
            _specialService = specialService;
        }

        private string GetCheckMessage(string charName, string attribName, int attribValue, int minimum)
        {
            if (attribValue < minimum)
            {
                return $"[{attribName} {attribValue}/{minimum}] Check **failed** for {charName}! ({Context.User.Mention})";
            }
            else
            {
                return $"[{attribName} {minimum}] Check **passed** for {charName}! ({Context.User.Mention})";
            }
        }

        [Command]
        public async Task<RuntimeResult> CheckStatistic(IUser user, Statistic stat, int minimum)
        {
            var character = await _charService.GetCharacterAsync(user.Id);

            if (character == null) return CharacterResult.CharacterNotFound();

            var match = character.Statistics.FirstOrDefault(x => x.Statistic.Equals(stat));
            if (match == null) return GenericResult.FromError("Statistic not found for character.");

            int statValue = match.Value;
            
            return GenericResult.FromSuccess(GetCheckMessage(character.Name, stat.Name, statValue, minimum));
        }
    }
}
