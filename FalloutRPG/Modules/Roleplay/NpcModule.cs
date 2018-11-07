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
        private readonly NpcService _npcService;
        private readonly NpcPresetService _presetService;
        private readonly HelpService _helpService;

        public NpcModule(NpcService npcService, NpcPresetService presetService, HelpService helpService)
        {
            _npcService = npcService;
            _presetService = presetService;
            _helpService = helpService;
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
            try
            {
                await _npcService.CreateNpc(name, type, level);
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
