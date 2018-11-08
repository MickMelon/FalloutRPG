using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("npc")]
    public class NpcModule : ModuleBase<SocketCommandContext>
    {
        private readonly CampaignService _campaignService;
        private readonly NpcService _npcService;
        private readonly NpcPresetService _presetService;
        private readonly HelpService _helpService;

        public NpcModule(CampaignService campaignService, NpcService npcService, NpcPresetService presetService, HelpService helpService)
        {
            _campaignService = campaignService;
            _helpService = helpService;
            _presetService = presetService;
            _npcService = npcService;
        }

        [Command("create")]
        [Alias("new")]
        public async Task CreateNewNpc(string type, string name)
        {
            await CreateNewNpc(type, name, 1);
        }

        [Command("create")]
        [Alias("new")]
        public async Task CreateNewNpc(string name, string type, int level)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            try
            {
                await _npcService.CreateNpc(name, type, level, campaign);
                await ReplyAsync(String.Format(Messages.NPC_CREATED_SUCCESS, type, name));
            }
            catch (Exception e)
            {
                await ReplyAsync(Messages.FAILURE_EMOJI + e.Message);
                return;
            }
        }

        [Command]
        [Alias("help")]
        public async Task ShowNpcHelp()
        {
            await _helpService.ShowNpcHelpAsync(Context);
        }
    }
}
