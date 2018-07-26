using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services
{
    public class HelpService
    {
        private readonly InteractiveService _interactiveService;

        public HelpService(InteractiveService interactiveService)
        {
            _interactiveService = interactiveService;
        }

        #region Index Help
        /// <summary>
        /// Shows the index help menu.
        /// </summary>
        public async Task ShowHelpAsync(SocketCommandContext context)
        {
            var userInfo = context.User;
            var embed = EmbedHelper.BuildBasicEmbedWithFields("Command: $help", string.Empty,
                Pages.HELP_PAGE1_TITLES, Pages.HELP_PAGE1_CONTENTS);

            await context.User.SendMessageAsync(userInfo.Mention, embed: embed);
        }
        #endregion

        #region General Help
        /// <summary>
        /// Shows the general help menu.
        /// </summary>
        public async Task ShowGeneralHelpAsync(SocketCommandContext context)
        {
            var userInfo = context.User;
            var embed = EmbedHelper.BuildBasicEmbedWithFields("Command: $help general", string.Empty,
                Pages.HELP_GENERAL_PAGE1_TITLES, Pages.HELP_GENERAL_PAGE1_CONTENTS);

            await context.User.SendMessageAsync(userInfo.Mention, embed: embed);
        }
        #endregion

        #region Character Help
        /// <summary>
        /// Shows the character help menu.
        /// </summary>
        public async Task ShowCharacterHelpAsync(SocketCommandContext context)
        {
            var page1 = PaginatedMessageHelper.BuildPageWithFields("Command: $help character",
                PaginatedMessageHelper.CreatePageFields(Pages.HELP_CHAR_PAGE1_TITLES, Pages.HELP_CHAR_PAGE1_CONTENTS));

            var page2 = PaginatedMessageHelper.BuildPageWithFields("Command: $help character",
                PaginatedMessageHelper.CreatePageFields(Pages.HELP_CHAR_PAGE2_TITLES, Pages.HELP_CHAR_PAGE2_CONTENTS));

            var pager = PaginatedMessageHelper.BuildPaginatedMessage(new[] { page1, page2 }, context.User);

            var reactions = new ReactionList
            {
                Forward = true,
                Backward = true,
                Jump = false,
                Trash = true
            };

            var callback = new CustomPaginatedMessageCallback(_interactiveService, context, pager);
            await callback.DisplayDMAsync(reactions).ConfigureAwait(false);
        }
        #endregion

        #region Roll Help
        /// <summary>
        /// Shows the roll help menu.
        /// </summary>
        public async Task ShowRollHelpAsync(SocketCommandContext context)
        {
            var userInfo = context.User;
            var embed = EmbedHelper.BuildBasicEmbedWithFields("Command: $help roll", string.Empty,
                Pages.HELP_ROLL_PAGE1_TITLES, Pages.HELP_ROLL_PAGE1_CONTENTS);

            await context.User.SendMessageAsync(userInfo.Mention, embed: embed);
        }
        #endregion

        #region Skills Help
        /// <summary>
        /// Shows the skills help menu.
        /// </summary>
        public async Task ShowSkillsHelpAsync(SocketCommandContext context)
        {
            var userInfo = context.User;
            var message = new StringBuilder();

            foreach (var skill in Globals.SKILL_NAMES)
                message.Append($"{skill}\n");

            var embed = EmbedHelper.BuildBasicEmbed("Command: $help skills", message.ToString());

            await context.User.SendMessageAsync(userInfo.Mention, embed: embed);
        }
        #endregion

        #region Craps Help
        /// <summary>
        /// Shows the Craps help menu.
        /// </summary>
        public async Task ShowCrapsHelpAsync(SocketCommandContext context)
        {
            var userInfo = context.User;
            var message = new StringBuilder();

            var embed = EmbedHelper.BuildBasicEmbedWithFields("Command: $help craps", string.Empty,
                Pages.HELP_CRAPS_PAGE1_TITLES, Pages.HELP_CRAPS_PAGE1_CONTENTS);

            await context.User.SendMessageAsync(userInfo.Mention, embed: embed);
        }
        #endregion

        #region Admin Help
        /// <summary>
        /// Shows the Admin help menu.
        /// </summary>
        public async Task ShowAdminHelpAsync(SocketCommandContext context)
        {
            var userInfo = context.User;
            var message = new StringBuilder();

            var embed = EmbedHelper.BuildBasicEmbedWithFields("Command: $help admin", string.Empty,
                Pages.HELP_ADMIN_PAGE1_TITLES, Pages.HELP_ADMIN_PAGE1_CONTENTS);

            await context.User.SendMessageAsync(userInfo.Mention, embed: embed);
        }
        #endregion
    }
}
