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
        public async Task CreateNewNpcAsync(string name, string type)
        {
            try
            {
                var preset = await _presetService.GetNpcPreset(type);

                if (preset == null)
                {
                    await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, Context.User.Mention));
                    return;
                }

                _npcService.CreateNpc(name, preset);
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
