using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Models.Configuration;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("character")]
    [Alias("char")]
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class SkillsModule : ModuleBase<SocketCommandContext>
    {
        [Group("skills")]
        [Alias("skill", "sk")]
        public class CharacterSkillsModule : ModuleBase<SocketCommandContext>
        {
            private readonly CharacterService _charService;
            private readonly EffectsService _effectsService;
            private readonly ExperienceService _expService;
            private readonly ProgressionOptions _progOptions;
            private readonly RoleplayOptions _roleplayOptions;
            private readonly SkillsService _skillsService;
            private readonly SpecialService _specService;
            private readonly StatisticsService _statsService;
            private readonly HelpService _helpService;

            public CharacterSkillsModule(
                CharacterService charService,
                EffectsService effectsService,
                ExperienceService expService,
                ProgressionOptions progOptions,
                RoleplayOptions roleplayOptions,
                SkillsService skillsService,
                SpecialService specService,
                StatisticsService statsService,
                HelpService helpService)
            {
                _charService = charService;
                _effectsService = effectsService;
                _expService = expService;
                _progOptions = progOptions;
                _roleplayOptions = roleplayOptions;
                _skillsService = skillsService;
                _specService = specService;
                _statsService = statsService;
                _helpService = helpService;
            }

            [Command]
            [Alias("show", "view")]
            public async Task<RuntimeResult> ShowSkillsAsync(IUser targetUser = null) =>
                await ShowSkillsAsync(targetUser, false);

            [Command("buffed")]
            [Alias("buff", "showb", "viewb")]
            public async Task<RuntimeResult> ShowSkillsBuffedAsync(IUser targetUser = null) =>
                await ShowSkillsAsync(targetUser, true);

            private async Task<RuntimeResult> ShowSkillsAsync(IUser targetUser = null, bool useEffects = false)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetCharacterAsync(userInfo.Id)
                    : await _charService.GetCharacterAsync(targetUser.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                bool skillsSet = _skillsService.AreSkillsSet(character);
                var stats = character.Statistics;
                if (useEffects)
                    stats = _effectsService.GetEffectiveStatistics(character);

                StringBuilder message = new StringBuilder($"**Name:** {character.Name}\n");

                foreach (var special in SpecialService.Specials.OrderBy(x => x.Id))
                {
                    message.Append($"__**{special.Name}:**__\n");
                    foreach (var skill in stats.Where(x => x.Statistic is Skill sk && sk.Special.Equals(special)).OrderBy(x => x.Statistic.Name))
                    {
                        if (skillsSet || !_progOptions.UseOldProgression)
                        {
                            if (skill.Value == 0) continue;

                            message.Append($"**{skill.Statistic.Name}:** {skill.Value}\n");
                        }
                        else
                        {
                            message.Append($"*{skill.Statistic.Name}*, ");
                        }
                    }

                    message.Append($"\n");
                }

                if (skillsSet)
                {
                    if (character.ExperiencePoints > 0)
                    {
                        message.Append($"*You have {character.ExperiencePoints}XP left to spend! ($char skills spend)*");
                    }
                }
                else
                {
                    if (!_progOptions.OldProgression.UseNewVegasRules)
                        message.Append($"*You have {character.TagPoints} Tag points left to spend!*");
                }

                var embed = EmbedHelper.BuildBasicEmbed("Command: $skills", message.ToString());

                await ReplyAsync(userInfo.Mention, embed: embed);
                return GenericResult.FromSilentSuccess();
            }

            [Command("help")]
            [Alias("help")]
            public async Task ShowSkillsHelpAsync()
            {
                await _helpService.ShowSkillsHelpAsync(Context);
            }

            [Command("init")]
            public async Task<RuntimeResult> InitializeStatisticsAsync()
            {
                if (!_progOptions.UseOldProgression ||
                    !_progOptions.OldProgression.UseNewVegasRules)
                    return StatisticResult.NotUsingNewVegasRules();

                var character = await _charService.GetCharacterAsync(Context.User.Id);
                if (character == null) return CharacterResult.CharacterNotFound();

                if (!_specService.IsSpecialSet(character)) return StatisticResult.SpecialNotSet();

                // Makes sure at least one skill has a value, otherwise the character might not have tagged their skills yet
                // We can't use !AreSkillsSet because it assumes that EVERY skill (including the brand new one) is greater than 0 at this point
                // If a new skill just got added, it's going to be 0 for everybody!
                if (character.Skills.Count(x => x.Value > 0) < 1) return StatisticResult.SkillsNotSet();

                // Now we check if all the skills count is the same, because if this is true, there aren't any new skills to initialize,
                // running this command is pointless and adds unnecessary risk
                if (character.Skills.Count == StatisticsService.Statistics.OfType<Skill>().Count())
                    return StatisticResult.SkillsAlreadyTagged();

                _statsService.InitializeStatistics(character.Statistics);
                _skillsService.InitializeSkills(character, true);
                await _charService.SaveCharacterAsync(character);

                return GenericResult.FromSuccess("Character skills initialized successfully.");
            }

            [Command("tag")]
            public async Task<RuntimeResult> TagSkillsAsync(Skill tag1, Skill tag2, Skill tag3)
            {
                if (!_progOptions.UseOldProgression || 
                    !_progOptions.OldProgression.UseNewVegasRules)
                    return StatisticResult.NotUsingNewVegasRules();

                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                if (_skillsService.AreSkillsSet(character)) return StatisticResult.SkillsAlreadyTagged();

                try
                {
                    await _skillsService.TagSkills(character, tag1, tag2, tag3);
                    return GenericResult.FromSuccess(Messages.SKILLS_SET_SUCCESS);
                }
                catch (Exception e)
                {
                    return GenericResult.FromError($"{Messages.FAILURE_EMOJI} {e.Message}");
                }
            }

            [Command("set")]
            [Alias("tag")]
            public async Task<RuntimeResult> SetSkillsAsync(Skill tag, int points)
            {
                if (ExperienceService.UseOldProgression) return StatisticResult.UsingOldProgression();

                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();
                if (_skillsService.AreSkillsSet(character)) return StatisticResult.SkillsAlreadyTagged();

                try
                {
                    await _skillsService.TagSkill(character, tag, points);
                    return GenericResult.FromSuccess(Messages.SKILLS_SET_SUCCESS);
                }
                catch (Exception e)
                {
                    return GenericResult.FromError($"{Messages.FAILURE_EMOJI} {e.Message}");
                }
            }

            [Command("spend")]
            [Alias("put", "upgrade")]
            public async Task<RuntimeResult> SpendSkillPointsAsync(Skill skill)
            {
                if (ExperienceService.UseOldProgression) return StatisticResult.UsingOldProgression();

                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();
                if (!_skillsService.AreSkillsSet(character)) return StatisticResult.SkillsNotSet();

                return _skillsService.UpgradeSkill(character, skill);
            }

            [Command("spend")]
            [Alias("put", "upgrade")]
            public async Task<RuntimeResult> SpendSkillPointsAsync(Skill skill, int points)
            {
                if (!ExperienceService.UseOldProgression) return StatisticResult.NotUsingOldProgression();

                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                if (!_skillsService.AreSkillsSet(character)) return StatisticResult.SkillsNotSet();

                if (points < 1) return GenericResult.FromError(Messages.ERR_SKILLS_POINTS_BELOW_ONE);

                if (points > character.ExperiencePoints)
                    return GenericResult.FromError(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

                var skillVal = _statsService.GetStatistic(character, skill);

                if ((skillVal + points) > _roleplayOptions.SkillMax)
                    return GenericResult.FromError(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

                _statsService.SetStatistic(character, skill, skillVal + points);
                character.ExperiencePoints -= points;

                await _charService.SaveCharacterAsync(character);

                return GenericResult.FromSuccess(Messages.SKILLS_SPEND_POINTS_SUCCESS);
            }

            [Command("claim")]
            public async Task<RuntimeResult> ClaimSkillPointsAsync()
            {
                if (!ExperienceService.UseOldProgression) return StatisticResult.NotUsingOldProgression();

                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();
                if (!character.IsReset) return GenericResult.FromError(Messages.ERR_SKILLS_NONE_TO_CLAIM);
                if (!_specService.IsSpecialSet(character)) return StatisticResult.SpecialNotSet();
                if (!_skillsService.AreSkillsSet(character)) return StatisticResult.SkillsNotSet();

                _statsService.InitializeStatistics(character.Statistics);

                character.ExperiencePoints = 0;
                var intelligence = _statsService.GetStatistic(character, Globals.StatisticFlag.Intelligence);

                for (int level = 1; level <= character.Level; level++)
                    character.ExperiencePoints += _expService.CalculateSkillPoints(level, intelligence);

                character.IsReset = false;
                await _charService.SaveCharacterAsync(character);

                return GenericResult.FromSuccess(string.Format(Messages.SKILLS_POINTS_CLAIMED, character.ExperiencePoints));
            }
        }
    }
}
