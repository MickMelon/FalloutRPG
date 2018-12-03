using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
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

                var skills = character.Skills;
                if (useEffects)
                    skills = _effectsService.GetEffectiveSkills(character);

                var embed = EmbedHelper.BuildBasicEmbed("Command: $skills",
                    $"**Name:** {character.Name}\n" +
                    $"**Barter:** {skills.Barter}\n" +
                    $"**Energy Weapons:** {skills.EnergyWeapons}\n" +
                    $"**Explosives:** {skills.Explosives}\n" +
                    $"**Guns:** {skills.Guns}\n" +
                    $"**Lockpick:** {skills.Lockpick}\n" +
                    $"**Medicine:** {skills.Medicine}\n" +
                    $"**Melee Weapons:** {skills.MeleeWeapons}\n" +
                    $"**Repair:** {skills.Repair}\n" +
                    $"**Science:** {skills.Science}\n" +
                    $"**Sneak:** {skills.Sneak}\n" +
                    $"**Speech:** {skills.Speech}\n" +
                    $"**Survival:** {skills.Survival}\n" +
                    $"**Unarmed:** {skills.Unarmed}\n" +
                    $"*You have {character.SkillPoints} left to spend! ($char skills spend)*");

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
            public async Task SetSkillsAsync(Globals.SkillType tag1, Globals.SkillType tag2, Globals.SkillType tag3)
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
            public async Task SpendSkillPointsAsync(Globals.SkillType skill, int points)
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
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null)
                {
                    await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (!character.IsReset)
                {
                    await ReplyAsync(string.Format(Messages.ERR_SKILLS_NONE_TO_CLAIM, userInfo.Mention));
                    return;
                }

                if (!_skillsService.AreSkillsSet(character))
                {
                    await ReplyAsync(string.Format(Messages.ERR_SKILLS_NOT_FOUND, userInfo.Mention));
                    return;
                }

                int pointsPerLevel = _skillsService.CalculateSkillPoints(character.Special.Intelligence);
                int totalPoints = pointsPerLevel * (character.Level - 1);

                if (totalPoints < 1)
                {
                    await ReplyAsync(string.Format(Messages.ERR_SKILLS_NONE_TO_CLAIM, userInfo.Mention));
                    return;
                }

                character.SkillPoints += totalPoints;
                character.IsReset = false;
                await _charService.SaveCharacterAsync(character);

                await ReplyAsync(string.Format(Messages.SKILLS_POINTS_CLAIMED, totalPoints, userInfo.Mention));
            }
        }
    }
}
