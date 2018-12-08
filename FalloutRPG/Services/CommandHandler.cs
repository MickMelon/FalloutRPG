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
            _commands.AddTypeReader(typeof(Globals.SkillType), new Addons.SkillTypeReader());
            _commands.AddTypeReader(typeof(Globals.SpecialType), new Addons.SpecialTypeReader());

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

        private async Task OnCommandExecutedAsync(CommandInfo command, ICommandContext context, IResult result)
        {
            if (result is PreconditionResult preResult)
            {
                await context.Channel.SendMessageAsync(preResult.ErrorReason);
            }
            else if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                if (result.Error.Value.ToString().Equals("ObjectNotFound") ||
                     result.Error.Value.ToString().Equals("BadArgCount"))
                {
                    await context.Channel.SendMessageAsync(string.Format(Messages.ERR_CMD_USAGE, context.User.Mention));
                }
                else if (result.Error.Value.ToString().Equals("UnknownCommand"))
                {
                    await context.Channel.SendMessageAsync(string.Format(Messages.ERR_CMD_NOT_EXIST, context.User.Mention));
                }
                else
                {
                    await context.Channel.SendMessageAsync(result.ToString());
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

                await OnCommandExecutedAsync(null, context, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
