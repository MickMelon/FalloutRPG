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
        private readonly CampaignService _campaignService;
        private readonly ItemService _itemService;
        private readonly HelpService _helpService;
        private readonly NpcPresetService _presetService;

        public NpcPresetModule(CampaignService campaignService,
            ItemService itemService,
            HelpService helpService,
            NpcPresetService presetService)
        {
            _campaignService = campaignService;
            _itemService = itemService;
            _helpService = helpService;
            _presetService = presetService;
        }

        [Command]
        [Alias("help")]
        public async Task ShowNpcPresetHelp()
        {
            await _helpService.ShowNpcPresetHelpAsync(Context);
        }

        [Command("add")]
        public async Task EditPresetInventoryAsync(string name, string itemName)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            var preset = await _presetService.GetNpcPreset(name, campaign);

            if (preset == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, Context.User.Mention));
                return;
            }

            var item = await _itemService.GetItemAsync(itemName);
            if (item == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_NOT_FOUND, Context.User.Mention));
                return;
            }

            preset.InitialInventory.Add(item);
            await _presetService.SaveNpcPreset(preset);

            await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT_INVENTORY, preset.Name, Context.User.Mention));
        }

        [Command("create")]
        public async Task CreatePresetAsync(string name)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            
            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            try
            {
                await _presetService.CreateNpcPresetAsync(name, campaign);
                await ReplyAsync(String.Format(Messages.NPC_PRESET_CREATE, name, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({Context.User.Mention})");
                throw;
            }
        }

        [Command("edit")]
        public async Task EditPresetSpecialAsync(string name, int str, int per, int end, int cha, int @int, int agi, int luc)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            var preset = await _presetService.GetNpcPreset(name, campaign);

            if (preset == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, Context.User.Mention));
                return;
            }

            preset.Special = new Special { Strength = str, Perception = per, Endurance = end, Intelligence = @int, Agility = agi, Luck = luc };
            await _presetService.SaveNpcPreset(preset);

            await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT_SPECIAL, preset.Name, Context.User.Mention));
        }

        [Command("edit")]
        public async Task EditPresetTagsAsync(string name, Globals.SkillType tag1, Globals.SkillType tag2, Globals.SkillType tag3)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            var preset = await _presetService.GetNpcPreset(name, campaign);

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
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            var preset = await _presetService.GetNpcPreset(name, campaign);

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
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await dmChannel.SendMessageAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            NpcPreset preset = await _presetService.GetNpcPreset(name, campaign);

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
