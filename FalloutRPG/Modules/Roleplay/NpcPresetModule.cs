using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("npc preset")]
    [Alias("npc pre")]
    public class NpcPresetModule : ModuleBase<SocketCommandContext>
    {
        private readonly HelpService _helpService;
        private readonly NpcPresetService _presetService;

        public NpcPresetModule(HelpService helpService, NpcPresetService presetService)
        {
            _helpService = helpService;
            _presetService = presetService;
        }

        [Command]
        [Alias("help")]
        public async Task ShowNpcPresetHelp()
        {
            await _helpService.ShowNpcPresetHelpAsync(Context);
        }

        [Command("create")]
        public async Task CreatePresetAsync(string name)
        {
            try
            {
                await _presetService.CreateNpcPresetAsync(name);
                await ReplyAsync(String.Format(Messages.NPC_PRESET_CREATE, name, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({Context.User.Mention})");
                throw;
            }
        }

        [Command("edit")]
        public async Task EditPresetSpecialAsync(string name, Special special, int newValue)
        {
            var preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, Context.User.Mention));
                return;
            }

            var match = preset.Special.Where(x => x.Equals(special)).FirstOrDefault();
            if (match != null) match.Value = newValue;

            await _presetService.SaveNpcPreset(preset);

            await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT_SPECIAL, preset.Name, Context.User.Mention));
        }

        [Command("edit")]
        public async Task EditPresetTagsAsync(string name, Skill tag1, Skill tag2, Skill tag3)
        {
            var preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, Context.User.Mention));
                return;
            }

            preset.Tag1 = tag1;
            preset.Tag2 = tag2;
            preset.Tag3 = tag3;
            
            await _presetService.SaveNpcPreset(preset);

            await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT_TAGS, preset.Name, Context.User.Mention));
        }

        [Command("toggle")]
        public async Task TogglePresetAsync(string name)
        {
            var preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, Context.User.Mention));
                return;
            }

            preset.Enabled = !preset.Enabled;
            await _presetService.SaveNpcPreset(preset);

            await ReplyAsync(String.Format(Messages.NPC_PRESET_TOGGLE, preset.Name, preset.Enabled, Context.User.Mention));
        }

        [Command("view")]
        public async Task ViewPresetInfo(string name)
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            NpcPreset preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
                await dmChannel.SendMessageAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, name, Context.User.Mention));

            StringBuilder sb = new StringBuilder();

            foreach (var prop in typeof(NpcPreset).GetProperties())
                if (Globals.SKILL_NAMES.Contains(prop.Name) || Globals.SPECIAL_NAMES.Contains(prop.Name) || prop.Name.Equals("Enabled") || prop.Name.Contains("Range"))
                    sb.Append($"{prop.Name}: {prop.GetValue(preset)}\n");

            await dmChannel.SendMessageAsync(Context.User.Mention, embed: EmbedHelper.BuildBasicEmbed($"Preset info for {preset.Name}:", sb.ToString()));
        }
    }
}
