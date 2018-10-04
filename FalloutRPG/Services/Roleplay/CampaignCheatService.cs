using System;
using System.Threading.Tasks;
using FalloutRPG.Constants;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;

public class CampaignCheatService
{
    private readonly PlayerService _playerService;
    private readonly CampaignService _campaignService;
    private readonly CharacterService _characterService;
    private readonly ExperienceService _experienceService;

    public async Task SetCharacterLevelAsync(ulong senderId, ulong receiverId, int level, ulong channelId)
    {
        if (level < 1)
            throw new ArgumentOutOfRangeException(Exceptions.LEVEL_TOO_LOW);

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

        int expForLevel = _experienceService.CalculateExperienceForLevel(level);
        if (expForLevel < character.Experience)
            throw new ArgumentOutOfRangeException(Exceptions.LEVEL_TOO_LOW);
        int difference = expForLevel - character.Experience;

        await _experienceService.GiveExperienceAsync(character, difference);
    }
}