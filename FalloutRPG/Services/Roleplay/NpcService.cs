using FalloutRPG.Constants;
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
        private readonly RollService _rollService;
        private readonly NpcPresetService _presetService;

        private readonly List<Character> Npcs;
        private readonly Dictionary<Character, Timer> NpcTimers;

        private readonly Random _rand;

        // Measured in seconds (not milliseconds):
        private const int NPC_ACTIVE_DURATION = 43200;
        
        public NpcService(SkillsService skillsService, RollService rollService, NpcPresetService presetService, IRepository<NpcPreset> presetRepository)
        {
            _rollService = rollService;
            _presetService = presetService;

            Npcs = new List<Character>();
            NpcTimers = new Dictionary<Character, Timer>();

            _rand = new Random();
        }

        public async Task CreateNpc(string npcType, string name)
        {
            if (Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null)
                throw new Exception(Exceptions.NPC_CHAR_EXISTS);

            NpcPreset preset = await _presetService.GetNpcPreset(npcType);

            if (preset == null)
                throw new Exception(Exceptions.NPC_INVALID_TYPE);
            if (preset.Enabled == false)
                throw new Exception(Exceptions.NPC_INVALID_TYPE_DISABLED);

            Character newNpc = new Character
            {
                Name = name,
                Special = new Special
                {
                    Strength     = preset.Strength     + _rand.Next(preset.StrengthRange     * -1, preset.StrengthRange     + 1),
                    Perception   = preset.Perception   + _rand.Next(preset.PerceptionRange   * -1, preset.PerceptionRange   + 1),
                    Endurance    = preset.Endurance    + _rand.Next(preset.EnduranceRange    * -1, preset.EnduranceRange    + 1),
                    Charisma     = preset.Charisma     + _rand.Next(preset.CharismaRange     * -1, preset.CharismaRange     + 1),
                    Intelligence = preset.Intelligence + _rand.Next(preset.IntelligenceRange * -1, preset.IntelligenceRange + 1),
                    Agility      = preset.Agility      + _rand.Next(preset.AgilityRange      * -1, preset.AgilityRange      + 1),
                    Luck         = preset.Luck         + _rand.Next(preset.LuckRange         * -1, preset.LuckRange         + 1)
                },
                Skills = new SkillSheet
                {
                    Barter        = preset.Barter        + _rand.Next(preset.BarterRange        * -1, preset.BarterRange        + 1),
                    EnergyWeapons = preset.EnergyWeapons + _rand.Next(preset.EnergyWeaponsRange * -1, preset.EnergyWeaponsRange + 1),
                    Explosives    = preset.Explosives    + _rand.Next(preset.ExplosivesRange    * -1, preset.ExplosivesRange    + 1),
                    Guns          = preset.Guns          + _rand.Next(preset.GunsRange          * -1, preset.GunsRange          + 1),
                    Lockpick      = preset.Lockpick      + _rand.Next(preset.LockpickRange      * -1, preset.LockpickRange      + 1),
                    Medicine      = preset.Medicine      + _rand.Next(preset.MedicineRange      * -1, preset.MedicineRange      + 1),
                    MeleeWeapons  = preset.MeleeWeapons  + _rand.Next(preset.MeleeWeaponsRange  * -1, preset.MeleeWeaponsRange  + 1),
                    Repair        = preset.Repair        + _rand.Next(preset.RepairRange        * -1, preset.RepairRange        + 1),
                    Science       = preset.Science       + _rand.Next(preset.ScienceRange       * -1, preset.ScienceRange       + 1),
                    Sneak         = preset.Sneak         + _rand.Next(preset.SneakRange         * -1, preset.SneakRange         + 1),
                    Speech        = preset.Speech        + _rand.Next(preset.SpeechRange        * -1, preset.SpeechRange        + 1),
                    Survival      = preset.Survival      + _rand.Next(preset.SurvivalRange      * -1, preset.SurvivalRange      + 1),
                    Unarmed       = preset.Unarmed       + _rand.Next(preset.UnarmedRange       * -1, preset.UnarmedRange       + 1)
                }
            };

            var timer = new Timer();
            timer.Elapsed += (sender, e) => OnDurationElasped(sender, e, newNpc);
            timer.Interval = NPC_ACTIVE_DURATION * 1000;
            timer.Enabled = true;

            NpcTimers.Add(newNpc, timer);

            Npcs.Add(newNpc);
        }

        public Character FindNpc(string name) => Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public string RollNpcStat(string name, Globals.SpecialType stat)
        {
            var character = Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (character == null)
                return String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, name);
            else if (character.Skills == null)
                throw new Exception(Exceptions.CHAR_SKILLS_NOT_SET);
            else if (character.Special == null)
                throw new Exception(Exceptions.CHAR_SPECIAL_NOT_SET);

            int attribAmt = (int)typeof(Special).GetProperty(stat.ToString()).GetValue(character.Special);

            if (attribAmt == 0)
                return String.Format(Messages.NPC_CANT_USE_SPECIAL, character.Name);

            ResetNpcTimer(character);

            return _rollService.GetRollMessage(character.Name, stat.ToString(), _rollService.GetRollResult(stat, character)) + " " + Messages.NPC_SUFFIX;
        }

        public string RollNpcStat(string name, Globals.SkillType stat)
        {
            var character = Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (character == null)
                return String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, name);
            else if (character.Skills == null)
                throw new Exception(Exceptions.CHAR_SKILLS_NOT_SET);
            else if (character.Special == null)
                throw new Exception(Exceptions.CHAR_SPECIAL_NOT_SET);

            int attribAmt = (int)typeof(SkillSheet).GetProperty(stat.ToString()).GetValue(character.Skills);

            if (attribAmt == 0)
                return String.Format(Messages.NPC_CANT_USE_SKILL, character.Name);

            ResetNpcTimer(character);

            return _rollService.GetRollMessage(character.Name, stat.ToString(), _rollService.GetRollResult(stat, character)) + " " + Messages.NPC_SUFFIX;
        }

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
