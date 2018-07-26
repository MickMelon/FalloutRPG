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
        private readonly HelpService _helpService;

        public NpcModule(NpcService npcService, HelpService helpService)
        {
            _npcService = npcService;
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
            NpcPreset preset = await _npcService.GetNpcPreset(type);

            await ReplyAsync(String.Format(Messages.NPC_CREATED_SUCCESS, preset.Name, name));
        }

        [Command]
        [Alias("help")]
        public async Task ShowCharacterHelpAsync()
        {
            await _helpService.ShowNpcHelpAsync(Context);
        }

        #region NPC Roll Commands
        [Group("roll")]
        public class NpcRollModule : ModuleBase<SocketCommandContext>
        {
            private readonly NpcService _npcService;

            public NpcRollModule(NpcService npcService)
            {
                _npcService = npcService;
            }

            #region SPECIAL Commands
            [Command("strength")]
            [Alias("str")]
            public async Task RollStrength(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Strength")}");

            [Command("perception")]
            [Alias("per")]
            public async Task RollPerception(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Perception")}");

            [Command("endurance")]
            [Alias("end")]
            public async Task RollEndurance(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Endurance")}");

            [Command("charisma")]
            [Alias("cha")]
            public async Task RollCharisma(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Charisma")}");

            [Command("intelligence")]
            [Alias("int")]
            public async Task RollIntelligence(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Intelligence")}");

            [Command("agility")]
            [Alias("agi")]
            public async Task RollAgility(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Agility")}");

            [Command("luck")]
            [Alias("luc", "lck")]
            public async Task RollLuck(string name) => await ReplyAsync($"{_npcService.RollNpcSpecial(name, "Luck")}");
            #endregion

            #region Skills Commands
            [Command("barter")]
            public async Task RollBarter(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Barter")}");

            [Command("energy weapons")]
            [Alias("energy weapon", "energyweapons", "energyweapon", "energy")]
            public async Task RollEnergyWeapons(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "EnergyWeapons")}");

            [Command("explosives")]
            public async Task RollExplosives(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Explosives")}");

            [Command("guns")]
            public async Task RollGuns(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Guns")}");

            [Command("lockpick")]
            public async Task RollLockpick(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Lockpick")}");

            [Command("medicine")]
            [Alias("medic", "doctor")]
            public async Task RollMedicine(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Medicine")}");

            [Command("meleeweapons")]
            [Alias("melee", "meleeweapon", "melee weapons", "melee weapons")]
            public async Task RollMeleeWeapons(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "MeleeWeapons")}");

            [Command("repair")]
            public async Task RollRepair(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Repair")}");

            [Command("science")]
            public async Task RollScience(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Science")}");

            [Command("sneak")]
            public async Task RollSneak(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Sneak")}");

            [Command("speech")]
            public async Task RollSpeech(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Speech")}");

            [Command("survival")]
            public async Task RollSurvival(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Survival")}");

            [Command("unarmed")]
            public async Task RollUnarmed(string name) => await ReplyAsync($"{_npcService.RollNpcSkill(name, "Unarmed")}");
            #endregion
        }
        #endregion

        #region NPC Preset Commands
        [Group("preset")]
        [Alias("pre")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public class NpcPresetModule : ModuleBase<SocketCommandContext>
        {
            private readonly NpcService _npcService;

            public NpcPresetModule(NpcService npcService)
            {
                _npcService = npcService;
            }

            [Command("create")]
            public async Task CreatePreset(string name)
            {
                if (await _npcService.CreateNpcPreset(name))
                    await ReplyAsync(String.Format(Messages.NPC_PRESET_CREATE, name, Context.User.Mention));
                else
                    await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_CREATE, Context.User.Mention));
            }

            [Command("create")]
            public async Task CreatePreset(string name, int str, int per, int end, int cha, int @int, int agi, int luc)
            {
                if (await _npcService.CreateNpcPreset(name, str, per, end, cha, @int, agi, luc, true))
                    await ReplyAsync(String.Format(Messages.NPC_PRESET_CREATE, name, Context.User.Mention));
                else
                    await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_CREATE, Context.User.Mention));
            }

            [Command("enable")]
            public async Task EnablePreset(string name)
            {
                if (await _npcService.EditNpcPresetEnable(name, true))
                    await ReplyAsync(String.Format(Messages.NPC_PRESET_ENABLE, name, Context.User.Mention));
                else
                    await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_ENABLE, name, Context.User.Mention));
            }

            [Command("disable")]
            public async Task DisablePreset(string name)
            {
                if (await _npcService.EditNpcPresetEnable(name, false))
                    await ReplyAsync(String.Format(Messages.NPC_PRESET_DISABLE, name, Context.User.Mention));
                else
                    await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_DISABLE, name, Context.User.Mention));
            }

            [Command("edit")]
            public async Task EditPreset(string name, string attribute, int value)
            {
                if (await _npcService.EditNpcPreset(name, attribute, value))
                    await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT, StringHelper.ToTitleCase(name), StringHelper.ToTitleCase(attribute), value, Context.User.Mention));
                else
                    await ReplyAsync(String.Format(Messages.ERR_NPC_PRESET_EDIT, Context.User.Mention));
            }

            [Command("edit")]
            public async Task EditPreset(string name, int str, int per, int end, int cha, int @int, int agi, int luc)
            {
                await _npcService.EditNpcPreset(name, "Strength", str);
                await _npcService.EditNpcPreset(name, "Perception", per);
                await _npcService.EditNpcPreset(name, "Endurance", end);
                await _npcService.EditNpcPreset(name, "Charisma", cha);
                await _npcService.EditNpcPreset(name, "Intelligence", @int);
                await _npcService.EditNpcPreset(name, "Agility", agi);
                await _npcService.EditNpcPreset(name, "Luck", luc);
                await ReplyAsync(String.Format(Messages.NPC_PRESET_EDIT_SPECIAL, name, Context.User.Mention));
            }

            [Command("initialize")]
            [Alias("init")]
            public async Task InitializePresetSkills(string name)
            {
                NpcPreset preset = await _npcService.GetNpcPreset(name);

                _npcService.InitializeNpcPresetSkills(preset);

                await _npcService.SaveNpcPreset(preset);

                await ReplyAsync(String.Format(Messages.NPC_PRESET_SKILLS_INIT, name, Context.User.Mention));
            }

            [Command("view")]
            public async Task ViewPresetInfo(string name)
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

                NpcPreset preset = await _npcService.GetNpcPreset(name);

                if (preset == null)
                    await dmChannel.SendMessageAsync(String.Format(Messages.ERR_NPC_PRESET_NOT_FOUND, name, Context.User.Mention));

                StringBuilder sb = new StringBuilder();

                foreach (var prop in typeof(NpcPreset).GetProperties())
                    sb.Append($"{prop.Name}: {prop.GetValue(preset)}\n");

                await dmChannel.SendMessageAsync(Context.User.Mention, embed: EmbedHelper.BuildBasicEmbed("Preset info:", sb.ToString()));
            }
        }
        #endregion
    }
}
