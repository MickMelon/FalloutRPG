﻿using Discord;
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
        private readonly StatisticsService _statsService;

        public NpcPresetModule(HelpService helpService, NpcPresetService presetService, StatisticsService statsService)
        {
            _helpService = helpService;
            _presetService = presetService;
            _statsService = statsService;
        }

        [Command]
        [Alias("help")]
        public async Task ShowNpcPresetHelp()
        {
            await _helpService.ShowNpcPresetHelpAsync(Context);
        }

        [Command("create")]
        public async Task<RuntimeResult> CreatePresetAsync(string name)
        {
            try
            {
                await _presetService.CreateNpcPresetAsync(name);
                return GenericResult.FromSuccess(String.Format(Messages.NPC_PRESET_CREATE, name));
            }
            catch (Exception e)
            {
                return GenericResult.FromError(e.Message);
                throw;
            }
        }

        [Command("edit")]
        public async Task<RuntimeResult> EditPresetStatisticAsync(string name, Statistic stat, int newValue)
        {
            var preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
                return GenericResult.FromError(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND));

            _statsService.SetStatistic(preset.Statistics, stat, newValue);
            await _presetService.SaveNpcPreset(preset);

            return GenericResult.FromSuccess(String.Format(Messages.NPC_PRESET_EDIT_SPECIAL, preset.Name));
        }

        [Command("toggle")]
        public async Task<RuntimeResult> TogglePresetAsync(string name)
        {
            var preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
                return GenericResult.FromError(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND));

            preset.Enabled = !preset.Enabled;
            await _presetService.SaveNpcPreset(preset);

            return GenericResult.FromSuccess(String.Format(Messages.NPC_PRESET_TOGGLE, preset.Name, preset.Enabled));
        }

        [Command("view")]
        public async Task ViewPresetInfo(string name)
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            NpcPreset preset = await _presetService.GetNpcPreset(name);

            if (preset == null)
                await dmChannel.SendMessageAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, name));

            StringBuilder sb = new StringBuilder();

            foreach (var stat in preset.Statistics)
                sb.Append($"{stat.Statistic.Name}: {stat.Value}\n");

            await dmChannel.SendMessageAsync(Context.User.Mention, embed: EmbedHelper.BuildBasicEmbed($"Preset info for {preset.Name}:", sb.ToString()));
        }
    }
}
