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
            private readonly SkillsService _skillsService;
            private readonly HelpService _helpService;

            public CharacterSkillsModule(
                CharacterService charService,
                SkillsService skillsService,
                HelpService helpService)
            {
                _charService = charService;
                _skillsService = skillsService;
                _helpService = helpService;
            }

            [Command]
            [Alias("show", "view")]
            public async Task ShowSkillsAsync(IUser targetUser = null)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetPlayerCharacterAsync(userInfo.Id)
                    : await _charService.GetPlayerCharacterAsync(targetUser.Id);

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

                var embed = EmbedHelper.BuildBasicEmbed("Command: $character skills",
                    $"**Name:** {character.Name}\n" +
                    $"**Barter:** {character.Skills.Barter}\n" +
                    $"**Energy Weapons:** {character.Skills.EnergyWeapons}\n" +
                    $"**Explosives:** {character.Skills.Explosives}\n" +
                    $"**Guns:** {character.Skills.Guns}\n" +
                    $"**Lockpick:** {character.Skills.Lockpick}\n" +
                    $"**Medicine:** {character.Skills.Medicine}\n" +
                    $"**MeleeWeapons:** {character.Skills.MeleeWeapons}\n" +
                    $"**Repair:** {character.Skills.Repair}\n" +
                    $"**Science:** {character.Skills.Science}\n" +
                    $"**Sneak:** {character.Skills.Sneak}\n" +
                    $"**Speech:** {character.Skills.Speech}\n" +
                    $"**Survival:** {character.Skills.Survival}\n" +
                    $"**Unarmed:** {character.Skills.Unarmed}\n" +
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
                var character = await _charService.GetPlayerCharacterAsync(userInfo.Id);

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
                var character = await _charService.GetPlayerCharacterAsync(userInfo.Id);

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
                var character = await _charService.GetPlayerCharacterAsync(userInfo.Id);

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
