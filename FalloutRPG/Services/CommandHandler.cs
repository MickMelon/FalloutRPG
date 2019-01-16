using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ExperienceService _expService;
        private readonly CharacterService _charService;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;

        public CommandHandler(
            DiscordSocketClient client,
            CommandService commands,
            ExperienceService expService,
            CharacterService charService,
            IServiceProvider services,
            IConfiguration config)
        {
            _client = client;
            _commands = commands;
            _expService = expService;
            _charService = charService;
            _services = services;
            _config = config;
        }

        /// <summary>
        /// Installs the commands and subscribes to MessageReceived event.
        /// </summary>
        public async Task InstallCommandsAsync()
        {
            _commands.AddTypeReader(typeof(Models.Statistic), new Helpers.StatisticTypeReader());
            _commands.AddTypeReader(typeof(Globals.StatisticFlag), new Helpers.StatisticFlagTypeReader());
            _commands.AddTypeReader(typeof(Models.Skill), new Helpers.SkillTypeReader());
            _commands.AddTypeReader(typeof(Models.Special), new Helpers.SpecialTypeReader());

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);

            #pragma warning disable CS1998
            _client.MessageReceived += async (message) =>
            #pragma warning restore CS1998
            {
                #pragma warning disable CS4014
                Task.Run(() => HandleCommandAsync(message));
                #pragma warning restore CS4014
            };

            _commands.CommandExecuted += OnCommandExecutedAsync;
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            string message = result.ToString();

            if (result is RuntimeResult rr && !String.IsNullOrEmpty(rr.Reason))
            {
                await context.Channel.SendMessageAsync(message);
            }

            else if (result is RollResult roll)
            {
                if (roll.RollEmbed != null) await context.Channel.SendMessageAsync(embed: roll.RollEmbed);
                if (!String.IsNullOrEmpty(roll.OldMessage)) await context.Channel.SendMessageAsync($"{roll.OldMessage} ({context.User.Mention})");
            }

            else if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    // CommandError.BadArgCount was getting thrown way too much because of the parameterless help commands
                    case CommandError.UnmetPrecondition: break;
                    case CommandError.UnknownCommand:
                        {
                            await context.Channel.SendMessageAsync(String.Format(Messages.ERR_CMD_NOT_EXIST, context.User.Mention));
                            break;
                        }
                    default:
                        {
                            await context.Channel.SendMessageAsync(String.Format(Messages.ERR_CMD_USAGE, context.User.Mention));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Handles incoming commands if it begins with specified prefix.
        /// If there is no prefix, it will process experience.
        /// </summary>
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            try
            {
                if (!(messageParam is SocketUserMessage message) || message.Author.IsBot) return;

                int argPos = 0;
                var context = new SocketCommandContext(_client, message);

                if (!(message.HasStringPrefix(_config["prefix"], ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                {
                    await _expService.ProcessExperienceAsync(context);
                    return;
                }

                var result = await _commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _services);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
