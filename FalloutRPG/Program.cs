using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using FalloutRPG.Data;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FalloutRPG
{
    public class Program
    {
        private IConfiguration config;

        /// <summary>
        /// The entry point of the program.
        /// </summary>
        public static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Initializes the services from the service provider.
        /// </summary>
        /// <remarks>
        /// await Task.Delay(-1) is required or else the program
        /// will end.
        /// </remarks>
        public async Task MainAsync()
        {
            config = BuildConfig();

            var services = BuildServiceProvider();

            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandler>().InstallCommandsAsync();
            await services.GetRequiredService<StartupService>().StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Builds the service provider for dependency injection.
        /// </summary>
        private IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton(config)
            .AddSingleton<CommandHandler>()
            .AddSingleton<LogService>()
            .AddSingleton<StartupService>()
            .AddSingleton<HelpService>()
            .AddSingleton<ReliabilityService>()

            // Roleplay
            .AddSingleton<CharacterService>()
            .AddSingleton<EffectsService>()
            .AddSingleton<ExperienceService>()
            .AddSingleton<ItemService>()
            .AddSingleton<RollService>()
            .AddSingleton<RollDiceService>()
            .AddSingleton<NpcService>()
            .AddSingleton<NpcPresetService>()
            .AddSingleton<PlayerService>()
            .AddSingleton<SkillsService>()
            .AddSingleton<SpecialService>()
            .AddSingleton<Random>()

            // Addons
            .AddSingleton<InteractiveService>()

            // Database
            .AddEntityFrameworkSqlite().AddDbContext<RpgContext>(optionsAction: options => options.UseSqlite("Filename=CharacterDB.db"))
            .AddTransient<IRepository<Player>, EfSqliteRepository<Player>>()
            .AddTransient<IRepository<Character>, EfSqliteRepository<Character>>()
            .AddTransient<IRepository<SkillSheet>, EfSqliteRepository<SkillSheet>>()
            .AddTransient<IRepository<Special>, EfSqliteRepository<Special>>()
            .AddTransient<IRepository<NpcPreset>, EfSqliteRepository<NpcPreset>>()
            .AddTransient<IRepository<Effect>, EfSqliteRepository<Effect>>()
            .AddTransient<IRepository<Item>, EfSqliteRepository<Item>>()
            .BuildServiceProvider();

        /// <summary>
        /// Builds the configuration from the Config.json file.
        /// </summary>
        private IConfiguration BuildConfig() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Config.json")
            .Build();
    }
}