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
            try
            {
                await _npcService.CreateNpc(type, name);
            }
            catch (Exception e)
            {
                await ReplyAsync(Messages.FAILURE_EMOJI + e.Message);
                return;
            }
            // used to show "Raider created" vs "raIDeR created" or whatever the user put in
            NpcPreset preset = await _presetService.GetNpcPreset(type);

            await ReplyAsync(String.Format(Messages.NPC_CREATED_SUCCESS, preset.Name, name));
        }

        [Command]
        [Alias("help")]
        public async Task ShowNpcHelp()
        {
            await _helpService.ShowNpcHelpAsync(Context);
        }

        [Group("roll")]
        public class NpcRollModule : ModuleBase<SocketCommandContext>
        {
            private readonly NpcService _npcService;

            public NpcRollModule(NpcService npcService)
            {
                _npcService = npcService;
            }

            public async Task RollSpecial(string name, Globals.SpecialType type) => await ReplyAsync(_npcService.RollNpcStat(name, type));

            public async Task RollSkill(string name, Globals.SkillType type) => await ReplyAsync(_npcService.RollNpcStat(name, type));
        }
    }
}
