using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Modules.Preconditions;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Modules
{
    [Group("admin")]
    [Alias("adm")]
    [RequireUserPermission(GuildPermission.BanMembers, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [RequireRole("FragsAdmin", Group = "Permission")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;
        private readonly ExperienceService _experienceService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly StatisticsService _statService;
        private readonly HelpService _helpService;

        public AdminModule(CharacterService charService,
            ExperienceService experienceService,
            SkillsService skillsService,
            SpecialService specialService,
            StatisticsService statService,
            HelpService helpService)
        {
            _charService = charService;
            _experienceService = experienceService;
            _skillsService = skillsService;
            _specialService = specialService;
            _statService = statService;
            _helpService = helpService;
        }

        [Command]
        public async Task ShowAdminHelpAsync()
        {
            await _helpService.ShowAdminHelpAsync(Context);
        }

        [Command("init")]
        public async Task<RuntimeResult> InitializeStatisticsAsync(IUser user)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            _statService.InitializeStatistics(character.Statistics);
            await _charService.SaveCharacterAsync(character);

            return GenericResult.FromSuccess("Character stats initialized successfully.");
        }

        [Command("addskill")]
        public async Task<RuntimeResult> AddSkillAsync(string name, Special special)
        {
            if (_statService.NameExists(name))
                return StatisticResult.StatisticAlreadyExists();

            var newSkill = new Skill
            {
                Name = name,
                Special = special,
                Aliases = name + "/"
            };

            await _statService.AddStatisticAsync(newSkill);         

            return GenericResult.FromSuccess(Messages.SKILLS_ADDED);
        }

        [Command("addspecial")]
        public async Task<RuntimeResult> AddSpecialAsync(string name)
        {
            if (_statService.NameExists(name))
                return StatisticResult.StatisticAlreadyExists();

            var newSpecial = new Special
            {
                Name = name,
                Aliases = name + "/"
            };

            await _statService.AddStatisticAsync(newSpecial);        

            return GenericResult.FromSuccess(Messages.SPECIAL_ADDED);
        }

        [Command("deletestat")]
        public async Task<RuntimeResult> DeleteStatAsync(Statistic stat)
        {
            await _statService.DeleteStatisticAsync(stat);

            return GenericResult.FromSuccess(Messages.SKILLS_REMOVED);
        }

        [Command("renamestat")]
        public async Task<RuntimeResult> RenameStatAsync(Statistic stat, string newName)
        {
            if (_statService.NameExists(newName))
                return StatisticResult.StatisticAlreadyExists();

            stat.Name = newName;
            stat.Aliases = newName + "/";

            await _statService.SaveStatisticAsync(stat);

            return GenericResult.FromSuccess("Statistic renamed successfully.");
        }

        [Command("addalias")]
        public async Task<RuntimeResult> AddAliasAsync(Statistic stat, string alias)
        {
            if (_statService.NameOrAliasExists(alias))
                return StatisticResult.StatisticAlreadyExists();

            stat.Aliases += alias + "/";

            await _statService.SaveStatisticAsync(stat);

            return GenericResult.FromSuccess("Alias added successfully.");
        }

        [Command("clearaliases")]
        public async Task<RuntimeResult> ClearAliasesAsync(Statistic stat, string alias)
        {
            stat.Aliases = stat.Name + "/";

            await _statService.SaveStatisticAsync(stat);

            return GenericResult.FromSuccess("Aliases cleared successfully.");
        }

        [Command("setdescription")]
        [Alias("setdesc")]
        public async Task<RuntimeResult> SetStatDescriptionAsync(Statistic stat, string desc)
        {
            stat.Description = desc;

            await _statService.SaveStatisticAsync(stat);

            return GenericResult.FromSuccess("Description set successfully.");
        }

        [Command("setflag")]
        public async Task<RuntimeResult> SetStatisticFlagAsync(Statistic stat, Globals.StatisticFlag flag)
        {
            stat.StatisticFlag = flag;

            await _statService.SaveStatisticAsync(stat);

            return GenericResult.FromSuccess("Flag set successfully.");
        }

        [Command("setminimum")]
        [Alias("setmin")]
        public async Task<RuntimeResult> SetSkillMinimumAsync(Skill skill, int min)
        {
            skill.MinimumValue = min;

            await _statService.SaveStatisticAsync(skill);

            return GenericResult.FromSuccess("Minimum set successfully.");
        }

        [Command("givemoney")]
        public async Task<RuntimeResult> GiveMoneyAsync(IUser user, int money)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            character.Money += money;

            await _charService.SaveCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.ADM_GAVE_MONEY);
        }

        [Command("giveexp")]
        public async Task<RuntimeResult> GiveExperienceAsync(IUser user, int points)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            await _experienceService.GiveExperienceAsync(character, points);

            await _charService.SaveCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.ADM_GAVE_EXP_POINTS);
        }

        [Command("giveskillpoints")]
        [RequireOwner]
        public async Task<RuntimeResult> GiveSkillPointsAsync(IUser user, int points)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            character.ExperiencePoints += points;

            await _charService.SaveCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.ADM_GAVE_SKILL_POINTS);
        }

        [Command("changename")]
        public async Task<RuntimeResult> ChangeCharacterNameAsync(IUser user, [Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            if (!StringHelper.IsOnlyLetters(name))
                return GenericResult.FromError("Name contained non-alphabetic characters.");

            if (name.Length > 24 || name.Length < 2)
                return GenericResult.FromError("Name was too long or short.");

            character.Name = StringHelper.ToTitleCase(name);

            await _charService.SaveCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.ADM_CHANGED_NAME);
        }
        
        [Command("reset")]
        public async Task<RuntimeResult> ResetCharacterAsync(IUser user)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            await _charService.ResetCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.ADM_RESET);
        }

        [Command("resetallcharacters")]
        [RequireOwner]
        public async Task<RuntimeResult> ResetAllCharactersAsync()
        {
            await _charService.ResetAllCharactersAsync();
            return GenericResult.FromSuccess(Messages.ADM_RESET);
        }

        [Command("delete")]
        [RequireOwner]
        public async Task<RuntimeResult> DeleteCharacterAsync(IUser user)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            await _charService.DeleteCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.ADM_DELETE);
        }
    }
}
