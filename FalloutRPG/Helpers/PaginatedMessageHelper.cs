using Discord;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalloutRPG.Helpers
{
    public class PaginatedMessageHelper
    {
        /// <summary>
        /// Build a new paginated message.
        /// </summary>
        public static PaginatedMessage BuildPaginatedMessage(PaginatedMessage.Page[] pages, IUser user)
        {
            var pager = new PaginatedMessage
            {
                Pages = pages,
                Color = Color.Blue,
                FooterOverride = null,
                Options = PaginatedAppearanceOptions.Default,
                Content = user.Mention
            };

            return pager;
        }

        /// <summary>
        /// Build a basic page for the paginated message.
        /// </summary>
        public static PaginatedMessage.Page BuildBasicPage(string title, string content)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                return null;

            return new PaginatedMessage.Page
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = title,
                },
                Description = content
            };
        }

        /// <summary>
        /// Build a page with fields for the paginated message.
        /// </summary>
        public static PaginatedMessage.Page BuildPageWithFields(string title, List<EmbedFieldBuilder> fields)
        {
            return new PaginatedMessage.Page
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = title,
                },
                Fields = fields
            };
        }

        /// <summary>
        /// Create a new field for paginated message.
        /// </summary>
        public static EmbedFieldBuilder CreateFieldBuilder(string title, string content)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                return null;

            return new EmbedFieldBuilder
            {
                Name = title,
                Value = content
            };
        }

        /// <summary>
        /// Create field list for paginated message.
        /// </summary>
        public static List<EmbedFieldBuilder> CreatePageFields(string[] titles, string[] contents)
        {
            if (titles.Length != contents.Length)
                throw new ArgumentException();

            var fields = new List<EmbedFieldBuilder>();

            for (var i = 0; i < titles.Length; i++)
                fields.Add(CreateFieldBuilder(titles[i], contents[i]));

            return fields;
        }

        public static List<EmbedFieldBuilder> CreatePageFields(
            ReadOnlyCollection<string> titles,
            ReadOnlyCollection<string> contents
        )
        {
            string[] titlesArray = new string[titles.Count];
            string[] contentsArray = new string[contents.Count];
            titles.CopyTo(titlesArray, 0);
            contents.CopyTo(contentsArray, 0);

            return CreatePageFields(titlesArray, contentsArray);
        }
    }
}
