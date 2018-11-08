﻿using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace FalloutRPG.Services.Roleplay
{
    public class NpcService
    {
        private readonly NpcPresetService _presetService;
        private readonly SkillsService _skillsService;

        private readonly List<Character> Npcs;
        private readonly Dictionary<Character, Timer> NpcTimers;

        private readonly Random _rand;

        private static readonly TimeSpan NPC_ACTIVE_DURATION = TimeSpan.FromHours(12);
        
        public NpcService(SkillsService skillsService,
            NpcPresetService presetService,
            IRepository<NpcPreset> presetRepository,
            Random random)
        {
            _presetService = presetService;
            _skillsService = skillsService;

            Npcs = new List<Character>();
            NpcTimers = new Dictionary<Character, Timer>();

            _rand = random;
        }

        public async Task CreateNpc(string name, string npcType, int level, Campaign campaign)
        {
            if (Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null)
                throw new Exception(Exceptions.NPC_CHAR_EXISTS);

            NpcPreset preset = await _presetService.GetNpcPreset(npcType, campaign);

            if (preset == null)
                throw new Exception(Exceptions.NPC_INVALID_PRESET);
            if (preset.Enabled == false)
                throw new Exception(Exceptions.NPC_INVALID_PRESET_DISABLED);

            Character newNpc = new NonPlayerCharacter { Name = name, Campaign = campaign, Special = preset.Special };

            _skillsService.InitializeSkills(newNpc);

            _skillsService.SetTagSkill(newNpc, preset.Tag1);
            _skillsService.SetTagSkill(newNpc, preset.Tag2);
            _skillsService.SetTagSkill(newNpc, preset.Tag3);

            var timer = new Timer();
            timer.Elapsed += (sender, e) => OnDurationElasped(sender, e, newNpc);
            timer.Interval = NPC_ACTIVE_DURATION.TotalMilliseconds;
            timer.Enabled = true;

            NpcTimers.Add(newNpc, timer);

            Npcs.Add(newNpc);
        }

        public Character FindNpc(string name) => Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Adds a user's Discord ID to the cooldowns.
        /// </summary>
        public void ResetNpcTimer(Character npc)
        {
            var timer = NpcTimers[npc];
            timer.Stop();
            timer.Start();
        }

        /// <summary>
        /// Called when a cooldown has finished.
        /// </summary>
        private void OnDurationElasped(object sender, ElapsedEventArgs e, Character npc)
        {
            var timer = NpcTimers[npc];
            timer.Enabled = false;
            timer.Dispose();

            NpcTimers.Remove(npc);
            Npcs.Remove(npc);
        }
    }
}
