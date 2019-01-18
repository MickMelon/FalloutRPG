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

        public static bool UseOldProgression { get; private set; } = false;
        public int DefaultSkillPoints { get; private set; } = 10;
        public bool UseNewVegasRules { get; private set; } = false;

        private const int DEFAULT_EXP_GAIN = 100;

        private readonly CharacterService _charService;
        private readonly StatisticsService _statService;
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;

        public ExperienceService(
            CharacterService charService,
            StatisticsService statService,
            DiscordSocketClient client,
            IConfiguration config,
            Random random)
        {
            _charService = charService;
            _statService = statService;

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

            // filter out messages sent by FRAGS
            var cache = context.Channel.GetCachedMessages(Math.Max(100, allowedConsecutiveMessages))
                .Where(x => !x.Author.ToString().Equals(context.Client.CurrentUser.ToString()))
                .OfType<SocketUserMessage>();

            // filter out messages trying to send commands
            int argPos = 0;
            cache = cache.Where(x => !(x.HasStringPrefix(_config["prefix"], ref argPos) ||
                    x.HasMentionPrefix(_client.CurrentUser, ref argPos)));   

            if (cache.Count() > allowedConsecutiveMessages &&
                cache.Take(allowedConsecutiveMessages).All(x => x.Author.Equals(context.User)))
                return;

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
            if (!UseOldProgression)
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
                int intStat = _statService.GetStatistic(character, Globals.StatisticFlag.Intelligence);

                if (intStat > 0)
                    expValue *= 1 + (intStat - intelligenceBaseline) * intelligenceMultiplier;
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
        /// Calculates the remaining experience required
        /// to get to the next level.
        /// </summary>
        /// <returns>
        /// Experience to next level or -1 if param is invalid.
        /// </returns>
        public int CalculateRemainingExperienceToNextLevel(int experience)
        {
            if (experience < 0) return -1;

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

        public int CalculateSkillPoints(int level, int intelligence)
        {
            int extraPoints = 0;

            if (UseNewVegasRules)
            {
                extraPoints = intelligence / 2;

                // When leveling up, the character distributes 10 + half Intelligence skill points.
                // (For odd intelligence scores, the "extra" skill point is given on even levels,
                // so a character with 1 intelligence will gain 11 skill points at level 2, then 10 at level 3, etc.)
                // http://fallout.wikia.com/wiki/Fallout:_New_Vegas_skills - 12/26/2018
                if (intelligence % 2 != 0 && level % 2 == 0)
                {
                    extraPoints += 1;
                }
            }

            return DefaultSkillPoints + extraPoints;
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

                UseOldProgression = _config
                    .GetValue<bool>("roleplay:experience:old-progression-system:enabled");

                DefaultSkillPoints = _config
                    .GetValue<int>("roleplay:experience:old-progression-system:skill-points-on-level-up");

                UseNewVegasRules = _config
                    .GetValue<bool>("roleplay:experience:old-progression-system:use-new-vegas-rules");

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

            if (UseOldProgression)
            {
                int originalLevel = character.Level - times;

                var intelligence = _statService.GetStatistic(character, Globals.StatisticFlag.Intelligence);

                for (int i = 1; i <= times; i++)
                    character.ExperiencePoints += CalculateSkillPoints(originalLevel + i, intelligence);

                await _charService.SaveCharacterAsync(character);
            }

            await user.SendMessageAsync(string.Format(Messages.SKILLS_LEVEL_UP, user.Mention, character.ExperiencePoints));
        }
    }
}
