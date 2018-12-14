using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Modules
{
    [Group("admin")]
    [Alias("adm")]
    [RequireUserPermission(GuildPermission.BanMembers, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
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

        [Command("addalias")]
        public RuntimeResult AddAlias(Statistic stat, string alias)
        {
            stat.Aliases += alias + "/";
            return GenericResult.FromSuccess("Alias added successfully.");
        }

        [Command("givemoney")]
        public async Task GiveMoneyAsync(IUser user, int money)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return;

            character.Money += money;

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_GAVE_MONEY, Context.User.Mention));
        }

        [Command("giveexp")]
        public async Task GiveExperienceAsync(IUser user, int points)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return;

            await _experienceService.GiveExperienceAsync(character, points);

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_GAVE_EXP_POINTS, Context.User.Mention));
        }

        [Command("giveskillpoints")]
        public async Task GiveSkillPointsAsync(IUser user, int points)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return;

            character.ExperiencePoints += points;

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_GAVE_SKILL_POINTS, Context.User.Mention));
        }

        [Command("changename")]
        public async Task ChangeCharacterNameAsync(IUser user, [Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return;

            if (!StringHelper.IsOnlyLetters(name))
                return;

            if (name.Length > 24 || name.Length < 2)
                return;

            character.Name = StringHelper.ToTitleCase(name);

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_CHANGED_NAME, Context.User.Mention));
        }
        
        [Command("reset")]
        public async Task ResetCharacterAsync(IUser user)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return;

            await _charService.ResetCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_RESET, Context.User.Mention));
        }

        [Command("delete")]
        [RequireOwner]
        public async Task DeleteCharacterAsync(IUser user)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return;

            await _charService.DeleteCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_DELETE, Context.User.Mention));
        }
    }
}
