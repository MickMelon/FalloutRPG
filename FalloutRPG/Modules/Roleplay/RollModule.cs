using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("roll")]
    [Alias("r")]
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly RollService _rollService;
        private readonly HelpService _helpService;

        public RollModule(RollService rollService, HelpService helpService)
        {
            _rollService = rollService;
            _helpService = helpService;
        }

        [Command]
        [Alias("help")]
        public async Task ShowRollHelpAsync()
        {
            await _helpService.ShowRollHelpAsync(Context);
        }

        [Command]
        public async Task RollSkill(Globals.SkillType skill)
        {
            await ReplyAsync($"{await _rollService.GetSkillRollAsync(Context.User, skill)} ({Context.User.Mention})");
        }
        [Command]
        public async Task RollSpecial(Globals.SpecialType special)
        {
            await ReplyAsync($"{await _rollService.GetSpRollAsync(Context.User, special)}  ({Context.User.Mention})");
        }
    }
}
