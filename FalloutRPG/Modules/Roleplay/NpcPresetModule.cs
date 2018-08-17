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
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class NpcPresetModule : ModuleBase<SocketCommandContext>
    {
        private readonly NpcPresetService _presetService;
        private readonly HelpService _helpService;

        public NpcPresetModule(NpcPresetService presetService, HelpService helpService)
        {
            _presetService = presetService;
            _helpService = helpService;
        }

        [Command]
        [Alias("help")]
        public async Task ShowNpcPresetHelp()
        {
            await _helpService.ShowNpcPresetHelpAsync(Context);
        }

        [Command("create")]
        public async Task CreatePreset(string name)
        {
            if (await _presetService.CreateNpcPreset(name))
                await ReplyAsync(String.Format(Messages.NPC_PRESET_CREATE, name, Context.User.Mention));
            else
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_CREATE, Context.User.Mention));
        }

        [Command("create")]
        public async Task CreatePreset(string name, int str, int per, int end, int cha, int @int, int agi, int luc)
        {
            if (await _presetService.CreateNpcPreset(name, str, per, end, cha, @int, agi, luc, true))
                await ReplyAsync(String.Format(Messages.NPC_PRESET_CREATE, name, Context.User.Mention));
            else
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_CREATE, Context.User.Mention));
        }

        [Command("enable")]
        public async Task EnablePreset(string name)
        {
            if (await _presetService.EditNpcPreset(name, "Enabled", true))
                await ReplyAsync(String.Format(Messages.NPC_PRESET_ENABLE, name, Context.User.Mention));
            else
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_ENABLE, name, Context.User.Mention));
        }

        [Command("disable")]
        public async Task DisablePreset(string name)
        {
            if (await _presetService.EditNpcPreset(name, "Enabled", false))
                await ReplyAsync(String.Format(Messages.NPC_PRESET_DISABLE, name, Context.User.Mention));
            else
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_DISABLE, name, Context.User.Mention));
        }

        [Command("edit")]
        public async Task EditPreset(string name, string attribute, int value)
        {
            if (attribute.Equals("Enabled", StringComparison.OrdinalIgnoreCase))
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_EDIT, Context.User.Mention));
                return;
            }
            if (await _presetService.EditNpcPreset(name, attribute, value))
                await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT, StringHelper.ToTitleCase(name), StringHelper.ToTitleCase(attribute), value, Context.User.Mention));
            else
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_EDIT, Context.User.Mention));
        }

        [Command("edit")]
        public async Task EditPreset(string name, int str, int per, int end, int cha, int @int, int agi, int luc)
        {
            await _presetService.EditNpcPreset(name, "Strength", str);
            await _presetService.EditNpcPreset(name, "Perception", per);
            await _presetService.EditNpcPreset(name, "Endurance", end);
            await _presetService.EditNpcPreset(name, "Charisma", cha);
            await _presetService.EditNpcPreset(name, "Intelligence", @int);
            await _presetService.EditNpcPreset(name, "Agility", agi);
            await _presetService.EditNpcPreset(name, "Luck", luc);
            await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT_SPECIAL, name, Context.User.Mention));
        }

        [Command("initialize")]
        [Alias("init")]
        public async Task InitializePresetSkills(string name)
        {
            NpcPreset preset = await _presetService.GetNpcPreset(name);

            _presetService.InitializeNpcPresetSkills(preset);

            await _presetService.SaveNpcPreset(preset);

            await ReplyAsync(String.Format(Messages.NPC_PRESET_SKILLS_INIT, name, Context.User.Mention));
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
            {
                if (Globals.SKILL_NAMES.Contains(prop.Name) || Globals.SPECIAL_NAMES.Contains(prop.Name) || prop.Name.Equals("Enabled"))
                    sb.Append($"{prop.Name}: {prop.GetValue(preset)}\n");
            }

            await dmChannel.SendMessageAsync(Context.User.Mention, embed: EmbedHelper.BuildBasicEmbed("Preset info:", sb.ToString()));
        }
    }
}
