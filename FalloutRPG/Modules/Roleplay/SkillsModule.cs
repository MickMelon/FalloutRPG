using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
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
            private readonly SkillsService _skillsService;
            private readonly HelpService _helpService;

            public CharacterSkillsModule(
                CharacterService charService,
                EffectsService effectsService,
                SkillsService skillsService,
                HelpService helpService)
            {
                _charService = charService;
                _effectsService = effectsService;
                _skillsService = skillsService;
                _helpService = helpService;
            }

            [Command]
            [Alias("show", "view")]
            public async Task ShowSkillsAsync(IUser targetUser = null) =>
                await ShowSkillsAsync(targetUser, false);

            [Command("buffed")]
            [Alias("buff", "showb", "viewb")]
            public async Task ShowSkillsBuffedAsync(IUser targetUser = null) =>
                await ShowSkillsAsync(targetUser, true);

            private async Task ShowSkillsAsync(IUser targetUser = null, bool useEffects = false)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetCharacterAsync(userInfo.Id)
                    : await _charService.GetCharacterAsync(targetUser.Id);

                if (character == null)
                {
                    await ReplyAsync(
                        string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (!_skillsService.AreSkillsSet(character))
                {
                    await ReplyAsync(
                        string.Format(Messages.ERR_SKILLS_NOT_FOUND, userInfo.Mention));
                    return;
                }

                var stats = character.Statistics;
                if (useEffects)
                    stats = _effectsService.GetEffectiveStatistics(character);

                StringBuilder message = new StringBuilder($"**Name:** {character.Name}\n");

                foreach (var skill in stats.Where(x => x.Statistic is Skill))
                    message.Append($"**{skill.Statistic.Name}:** {skill.Value}\n");

                message.Append($"*You have {character.ExperiencePoints}XP left to spend! ($char skills spend)*");

                var embed = EmbedHelper.BuildBasicEmbed("Command: $skills", message.ToString());

                await ReplyAsync(userInfo.Mention, embed: embed);
            }

            [Command("help")]
            [Alias("help")]
            public async Task ShowSkillsHelpAsync()
            {
                await _helpService.ShowSkillsHelpAsync(Context);
            }

            [Command("set")]
            [Alias("tag")]
            public async Task SetSkillsAsync(Skill tag1, Skill tag2, Skill tag3)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null)
                {
                    await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (_skillsService.AreSkillsSet(character))
                {
                    await ReplyAsync(string.Format(Messages.ERR_SKILLS_ALREADY_SET, userInfo.Mention));
                    return;
                }

                try
                {
                    await _skillsService.SetTagSkills(character, tag1, tag2, tag3);
                    await ReplyAsync(string.Format(Messages.SKILLS_SET_SUCCESS, userInfo.Mention));
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({userInfo.Mention})");
                }
            }

            [Command("spend")]
            [Alias("put")]
            public async Task SpendSkillPointsAsync(Skill skill, int points)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null)
                {
                    await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (!_skillsService.AreSkillsSet(character))
                {
                    await ReplyAsync(string.Format(Messages.ERR_SKILLS_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (points < 1)
                {
                    await ReplyAsync(string.Format(Messages.ERR_SKILLS_POINTS_BELOW_ONE, userInfo.Mention));
                    return;
                }

                try
                {
                    _skillsService.PutPointsInSkill(character, skill, points);
                    await ReplyAsync(string.Format(Messages.SKILLS_SPEND_POINTS_SUCCESS, userInfo.Mention));
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({userInfo.Mention})");
                }
            }

            [Command("claim")]
            public async Task ClaimSkillPointsAsync()
            {
                // TODO: ...obviously
                await Task.Delay(0);
                throw new NotImplementedException();

                // var userInfo = Context.User;
                // var character = await _charService.GetCharacterAsync(userInfo.Id);

                // if (character == null)
                // {
                //     await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                //     return;
                // }

                // if (!character.IsReset)
                // {
                //     await ReplyAsync(string.Format(Messages.ERR_SKILLS_NONE_TO_CLAIM, userInfo.Mention));
                //     return;
                // }

                // if (!_skillsService.AreSkillsSet(character))
                // {
                //     await ReplyAsync(string.Format(Messages.ERR_SKILLS_NOT_FOUND, userInfo.Mention));
                //     return;
                // }

                // int pointsPerLevel = 0;
                // //int pointsPerLevel = _skillsService.CalculateSkillPoints(character.Special.Intelligence);
                // int totalPoints = pointsPerLevel * (character.Level - 1);

                // if (totalPoints < 1)
                // {
                //     await ReplyAsync(string.Format(Messages.ERR_SKILLS_NONE_TO_CLAIM, userInfo.Mention));
                //     return;
                // }

                // character.SkillPoints += totalPoints;
                // character.IsReset = false;
                // await _charService.SaveCharacterAsync(character);

                // await ReplyAsync(string.Format(Messages.SKILLS_POINTS_CLAIMED, totalPoints, userInfo.Mention));
            }
        }
    }
}
