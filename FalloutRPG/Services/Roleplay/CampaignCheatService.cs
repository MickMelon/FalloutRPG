using System;
using System.Threading.Tasks;
using Discord;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;

public class CampaignCheatService
{
    private readonly CampaignService _campaignService;
    private readonly CharacterService _characterService;
    private readonly ExperienceService _experienceService;
    private readonly SkillsService _skillsService;
    private readonly SpecialService _specialService;
    private readonly PlayerService _playerService;

    private async Task<(bool isValid, Character character)> CheckValidityAsync(ulong channelId, ulong senderId, ulong receiverId)
    {
        var receiver = await _playerService.GetPlayerAsync(senderId);
        var moderator = await _playerService.GetPlayerAsync(receiverId);

        var campaign = await _campaignService.GetCampaignAsync(channelId);
        if (campaign == null)
            throw new Exception(Exceptions.CAMP_CHANNEL_COMMAND);
        if (!campaign.Moderators.Contains(moderator))
            throw new Exception(Exceptions.CAMP_NOT_MODERATOR);
        if (!campaign.Players.Contains(receiver))
            throw new Exception(Exceptions.CAMP_NOT_A_MEMBER);

        var character = await _characterService.GetPlayerCharacterAsync(receiver.DiscordId);

        if (character == null)
            throw new Exception(Exceptions.CHAR_NOT_FOUND);
        if (character.Campaign == null || !character.Campaign.Equals(campaign))
            throw new Exception(Exceptions.CAMP_CHAR_NOT_JOINED);

        return (true, character);
    }

    public async Task SetCharacterStatAsync(ulong channelId, ulong senderId, ulong receiverId, Globals.SkillType skill, int newValue)
    {
        if (newValue < 0)
            throw new ArgumentOutOfRangeException("newValue", Exceptions.LEVEL_TOO_LOW);

        var (isValid, character) = await CheckValidityAsync(channelId, senderId, receiverId);

        _skillsService.SetSkill(character, skill, newValue);
        await _characterService.SaveCharacterAsync(character);
    }

    public async Task SetCharacterStatAsync(ulong channelId, ulong senderId, ulong receiverId, Globals.SpecialType special, int newValue)
    {
        if (newValue < 0)
            throw new ArgumentOutOfRangeException("newValue", Exceptions.LEVEL_TOO_LOW);

        var (isValid, character) = await CheckValidityAsync(channelId, senderId, receiverId);

        _specialService.SetSpecial(character, special, newValue);
        await _characterService.SaveCharacterAsync(character);
    }

    public async Task SetCharacterSkillPointsAsync(ulong channelId, ulong senderId, ulong receiverId, int newValue)
    {
        if (newValue < 0)
            throw new ArgumentOutOfRangeException("newValue", Exceptions.LEVEL_TOO_LOW);

        var (isValid, character) = await CheckValidityAsync(channelId, senderId, receiverId);

        if (character is PlayerCharacter pc)
            pc.SkillPoints = newValue;
        else
            throw new Exception(Exceptions.CHAR_NOT_PLAYER);

        await _characterService.SaveCharacterAsync(character);
    }

    public async Task SetCharacterLevelAsync(ulong senderId, ulong receiverId, int level, ulong channelId)
    {
        if (level < 1)
            throw new ArgumentOutOfRangeException(Exceptions.LEVEL_TOO_LOW);

        var (isValid, character) = await CheckValidityAsync(channelId, senderId, receiverId);

        int expForLevel = _experienceService.CalculateExperienceForLevel(level);
        if (expForLevel < character.Experience)
            throw new ArgumentOutOfRangeException(Exceptions.LEVEL_TOO_LOW);
        int difference = expForLevel - character.Experience;

        await _experienceService.GiveExperienceAsync(character as PlayerCharacter, difference);
    }
}