using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using FalloutRPG.Services;
using System.Threading.Tasks;

namespace FalloutRPG.Modules
{
    [Group("help")]
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly HelpService _helpService;

        public HelpModule(HelpService helpService)
        {
            _helpService = helpService;
        }

        [Command]
        public async Task ShowHelpAsync()
        {
            await _helpService.ShowHelpAsync(Context);
        }

        [Group("general")]
        public class GeneralHelpModule : InteractiveBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public GeneralHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowGeneralHelpAsync()
            {
                await _helpService.ShowGeneralHelpAsync(Context);
            }
        }

        [Group("character")]
        public class CharacterHelpModule : InteractiveBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public CharacterHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowCharacterHelpAsync()
            {
                await _helpService.ShowCharacterHelpAsync(Context);
            }
        }

        [Group("roll")]
        public class RollHelpModule : InteractiveBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public RollHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowRollHelpAsync()
            {
                await _helpService.ShowRollHelpAsync(Context);
            }
        }

        [Group("skills")]
        [Alias("skill")]
        public class SkillsHelpModule : InteractiveBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public SkillsHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowSkillsHelpAsync()
            {
                await _helpService.ShowSkillsHelpAsync(Context);
            }

            [Command]
            public async Task ShowSkillsHelpAsync(Skill skill)
            {
                await _helpService.ShowSkillsHelpAsync(Context, skill);
            }
        }

        [Group("special")]
        [Alias("spec")]
        public class SpecialHelpModule : InteractiveBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public SpecialHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowSkillsHelpAsync()
            {
                await _helpService.ShowSpecialHelpAsync(Context);
            }

            [Command]
            public async Task ShowSkillsHelpAsync(Special spec)
            {
                await _helpService.ShowSpecialHelpAsync(Context, spec);
            }
        }

        [Group("craps")]
        public class CrapsHelpModule : ModuleBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public CrapsHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowCrapsHelpAsync()
            {
                await _helpService.ShowCrapsHelpAsync(Context);
            }
        }

        [Group("admin")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireOwner]
        public class AdminHelpModule : ModuleBase<SocketCommandContext>
        {
            private readonly HelpService _helpService;

            public AdminHelpModule(HelpService helpService)
            {
                _helpService = helpService;
            }

            [Command]
            public async Task ShowAdminHelpAsync()
            {
                await _helpService.ShowAdminHelpAsync(Context);
            }
        }
    }
}
