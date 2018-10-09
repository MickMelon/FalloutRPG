using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("camp")]
    public class CampaignCheatModule : ModuleBase<SocketCommandContext>
    {
        private readonly CampaignCheatService _cheatService;

        public CampaignCheatModule(CampaignCheatService cheatService)
        {
            _cheatService = cheatService;
        }

        [Command("edit")]
        public async Task EditCharacterAsync(IUser receiver, Globals.SkillType skill, int newValue)
        {
            try
            {
                await _cheatService.SetCharacterStatAsync(Context.Channel.Id, Context.User.Id, receiver.Id, skill, newValue);
                await ReplyAsync(String.Format(Messages.CHEAT_CHAR_EDIT, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({Context.User.Mention}");
                return;
            }
        }
        [Command("edit")]
        public async Task EditCharacterAsync(IUser receiver, Globals.SpecialType special, int newValue)
        {
            try
            {
                await _cheatService.SetCharacterStatAsync(Context.Channel.Id, Context.User.Id, receiver.Id, special, newValue);
                await ReplyAsync(String.Format(Messages.CHEAT_CHAR_EDIT, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({Context.User.Mention}");
                return;
            }
        }

        [Command("setskillpoints")]
        public async Task SetCharacterSkillPointsAsync(IUser receiver, int newValue)
        {
            try
            {
                await _cheatService.SetCharacterSkillPointsAsync(Context.Channel.Id, Context.User.Id, receiver.Id, newValue);
                await ReplyAsync(String.Format(Messages.CHEAT_SKILL_POINTS_GIVEN, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({Context.User.Mention}");
                return;
            }
        }

        [Command("setlevel")]
        public async Task SetLevelAsync(IUser user, int level)
        {
            try
            {
                await _cheatService.SetCharacterLevelAsync(Context.User.Id, user.Id, level, Context.Channel.Id);
                await ReplyAsync(String.Format(Messages.CHEAT_LEVEL_CHANGE, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message}");
                return;
            }
        }
    }
}
