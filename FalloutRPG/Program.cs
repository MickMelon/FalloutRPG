using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using FalloutRPG.Data;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Models.Configuration;
using FalloutRPG.Models.Effects;
using FalloutRPG.Services;
using FalloutRPG.Services.Casino;
using FalloutRPG.Services.Roleplay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FalloutRPG
{
    public class Program
    {
        private IConfiguration config;

        /// <summary>
        /// The entry point of the program
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
            // Config
            .AddSingleton(config)
            .Configure<GeneralOptions>(config)
            .Configure<GamblingOptions>(config.GetSection("Gambling"))
            .Configure<RoleplayOptions>(config.GetSection("Roleplay"))
            .Configure<ChargenOptions>(config.GetSection("Roleplay:Chargen"))
            .Configure<ExperienceOptions>(config.GetSection("Roleplay:Experience"))
            .Configure<ProgressionOptions>(config.GetSection("Roleplay:Progression"))
            .Configure<TokensOptions>(config.GetSection("Tokens"))

            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<GeneralOptions>>().Value)
            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<GamblingOptions>>().Value)
            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<RoleplayOptions>>().Value)
            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ChargenOptions>>().Value)
            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ExperienceOptions>>().Value)
            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ProgressionOptions>>().Value)
            .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TokensOptions>>().Value)

            .AddScoped<CommandHandler>()
            .AddSingleton<LogService>()
            .AddSingleton<StartupService>()
            .AddSingleton<HelpService>()
            .AddSingleton<ReliabilityService>()

            // Roleplay
            .AddSingleton<Random>()
            .AddSingleton<RollService>()
            .AddSingleton<StatisticsService>()
            .AddSingleton<SkillsService>()
            .AddSingleton<SpecialService>()
            .AddSingleton<StartupService>()
            .AddTransient<CharacterService>()
            .AddTransient<ExperienceService>()
            .AddSingleton<EffectsService>()
            .AddSingleton<NpcPresetService>()
            .AddSingleton<NpcService>()

            // Casino
            .AddScoped<GamblingService>()
            .AddSingleton<CrapsService>()

            // Addons
            .AddSingleton<InteractiveService>()

            // Database
            .AddEntityFrameworkSqlite().AddDbContext<RpgContext>(optionsAction: options => options.UseSqlite("Filename=CharacterDB.db"))
            .AddTransient<IRepository<Character>, EfSqliteRepository<Character>>()
            .AddTransient<IRepository<Statistic>, EfSqliteRepository<Statistic>>()
            .AddTransient<IRepository<Effect>, EfSqliteRepository<Effect>>()
            .AddTransient<IRepository<NpcPreset>, EfSqliteRepository<NpcPreset>>()
            .BuildServiceProvider();

        /// <summary>
        /// Builds the configuration from the Config.json file.
        /// </summary>
        private IConfiguration BuildConfig() => new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("Config.json", false, true)
            .Build();
    }
}
