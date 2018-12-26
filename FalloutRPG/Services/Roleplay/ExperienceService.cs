using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace FalloutRPG.Services.Roleplay
{
    public class ExperienceService
    {
        private List<ulong> experienceEnabledChannels;
        private readonly Random _random;

        private bool intelligenceEnabled;
        private int intelligenceBaseline;
        private double intelligenceMultiplier;

        private double messageLengthDivisor;
        private int allowedConsecutiveMessages;

        private bool priceIncreaseEnabled;
        private double priceIncreaseMultiplierAddition;
        private int priceIncreaseStartingLevel;
        private int priceIncreaseEveryXLevels;

        private const int DEFAULT_EXP_GAIN = 100;

        private readonly CharacterService _charService;
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;

        public ExperienceService(
            CharacterService charService,
            DiscordSocketClient client,
            IConfiguration config,
            Random random)
        {
            _charService = charService;
            _client = client;
            _config = config;
            
            LoadExperienceConfig();
            _random = random;
        }

        /// <summary>
        /// Processes experience to give if channel is an experience
        /// enabled channel.
        /// </summary>
        public async Task ProcessExperienceAsync(SocketCommandContext context)
        {
            if (!IsInExperienceEnabledChannel(context.Channel.Id)) return;

            var userInfo = context.User;
            var character = await _charService.GetCharacterAsync(userInfo.Id);

            if (character == null || context.Message.ToString().StartsWith("(")) return;

            // filter out users abusing monologues for exp
            var cache = context.Channel.GetCachedMessages(allowedConsecutiveMessages)
                .Where(x => !x.Author.Equals(context.Client.CurrentUser));
            if (cache.All(x => x.Author.Equals(context.User))) return;

            var expToGive = GetExperienceFromMessage(character, context.Message.Content.Where(x => !Char.IsWhiteSpace(x)).Count());

            if (await GiveExperienceAsync(character, expToGive))
            {
                var level = CalculateLevelForExperience(character.Experience);
                await context.Channel.SendMessageAsync(
                    string.Format(Messages.EXP_LEVEL_UP, userInfo.Mention, level));
            }
        }

        /// <summary>
        /// Gives experience to a character.
        /// </summary>
        public async Task<bool> GiveExperienceAsync(Character character, int experience = DEFAULT_EXP_GAIN)
        {
            if (character == null) return false;

            var initialLevel = character.Level;

            character.Experience += experience;
            character.ExperiencePoints += experience;
            await _charService.SaveCharacterAsync(character);

            var levelUp = false;
            var difference = character.Level - initialLevel;

            if (difference >= 1)
            {
                await OnLevelUpAsync(character, difference);
                levelUp = true;
            }
            
            return levelUp;
        }

        public int GetExperienceFromMessage(Character character, int messageLength)
        {
            double expValue = Math.Round(Math.Max(1, messageLength / messageLengthDivisor));

            if (intelligenceEnabled)
            {
                var intStat = character.Statistics.Where(x => x.Statistic.StatisticFlag == Globals.StatisticFlag.Intelligence).FirstOrDefault();

                if (intStat != null)
                    expValue *= (int)(1 + (intStat.Value - intelligenceBaseline) * intelligenceMultiplier);
            }

            return (int)Math.Round(expValue);
        }

        /// <summary>
        /// Calculate the experience required for a level.
        /// </summary>
        public int CalculateExperienceForLevel(int level)
        {
            if (level < 1 || level > 1000) return -1;
            return (level * (level - 1) / 2) * 1000;
        }

        /// <summary>
        /// Calculates experience between the beginning of
        /// one level and the next.
        /// </summary>
        public int CalculateExperienceToNextLevel(int level)
        {
            if (level < 1 || level > 1000) return -1;
            return 50 + (150 * level);
        }

        /// <summary>
        /// Calculates the remaining experience required
        /// to get to the next level.
        /// </summary>
        public int CalculateRemainingExperienceToNextLevel(int experience)
        {
            var nextLevel = (CalculateLevelForExperience(experience) + 1);
            var nextLevelExp = CalculateExperienceForLevel(nextLevel);
            return (nextLevelExp - experience);
        }

        /// <summary>
        /// Calculates the level depending on the experience.
        /// </summary>
        public int CalculateLevelForExperience(int experience)
        {
            if (experience == 0) return 1;
            return Convert.ToInt32((Math.Sqrt(experience + 125) / (10 * Math.Sqrt(5))));
        }

        /// <summary>
        /// Checks if the input Channel ID is an experience
        /// enabled channel.
        /// </summary>
        public bool IsInExperienceEnabledChannel(ulong channelId)
        {
            foreach (var channel in experienceEnabledChannels)
                if (channelId == channel)
                    return true;

            return false;
        }

        public double GetPriceMultiplier(int level)
        {
            double multiplier = 1.0;

            if (priceIncreaseEnabled)
            {
                level -= priceIncreaseStartingLevel;

                for (int i = 0; i <= level; i += priceIncreaseEveryXLevels)
                    multiplier += priceIncreaseMultiplierAddition;
            }

            return multiplier;
        }

        /// <summary>
        /// Loads the experience enabled channels from the
        /// configuration file.
        /// </summary>
        private void LoadExperienceConfig()
        {
            try
            {
                experienceEnabledChannels = _config
                    .GetSection("roleplay:experience:exp-channels")
                    .GetChildren()
                    .Select(x => UInt64.Parse(x.Value))
                    .ToList();

                intelligenceEnabled = _config
                    .GetValue<bool>("roleplay:experience:intelligence-based-exp-gain:enabled");

                intelligenceBaseline = _config
                    .GetValue<int>("roleplay:experience:intelligence-based-exp-gain:baseline");
                
                intelligenceMultiplier = _config
                    .GetValue<double>("roleplay:experience:intelligence-based-exp-gain:multiplier");

                allowedConsecutiveMessages = _config
                    .GetValue<int>("roleplay:experience:allowed-consecutive-messages");

                messageLengthDivisor = _config
                    .GetValue<double>("roleplay:experience:message-length-divisor");

                priceIncreaseEnabled = _config
                    .GetValue<bool>("roleplay:experience:price-increase:enabled");

                priceIncreaseStartingLevel = _config
                    .GetValue<int>("roleplay:experience:price-increase:starting-level");

                priceIncreaseEveryXLevels = _config
                    .GetValue<int>("roleplay:experience:price-increase:increase-every");

                priceIncreaseMultiplierAddition = _config
                    .GetValue<double>("roleplay:experience:price-increase:multiplier-addition");
            }
            catch (Exception)
            {
                Console.WriteLine("Experience settings improperly configured, Config.json.");
            }
        }

        /// <summary>
        /// Checks if adding the experience will result in a level up.
        /// </summary>
        private bool WillLevelUp(Character character, int expToAdd)
        {
            if (character == null) throw new ArgumentNullException("character");
            int nextLevelExp = CalculateExperienceForLevel(CalculateLevelForExperience(character.Experience) + 1);
            return (character.Experience + expToAdd) >= nextLevelExp;
        }

        /// <summary>
        /// Called when a character levels up.
        /// </summary>
        private async Task OnLevelUpAsync(Character character, int times = 1)
        {
            if (character == null) throw new ArgumentNullException("character");
            var user = _client.GetUser(character.DiscordId);

            await user.SendMessageAsync(string.Format(Messages.SKILLS_LEVEL_UP, user.Mention, character.ExperiencePoints));
        }
    }
}
