using System.Globalization;
using System.Text.RegularExpressions;

namespace FalloutRPG.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Shortens a string to the specified length.
        /// </summary>
        public static string Truncate(string source, int length)
        {
            if (source.Length > length)
                source = source.Substring(0, length);

            return source;
        }

        /// <summary>
        /// Checks if a string contains only letters.
        /// </summary>
        public static bool IsOnlyLetters(string source)
        {
            return Regex.IsMatch(source, @"^[a-zA-Z ]+$");
        }

        /// <summary>
        /// Capitalizes the first letter in the source string.
        /// </summary>
        public static string UppercaseFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            return char.ToUpper(source[0]) + source.Substring(1);
        }

        /// <summary>
        /// Sets source string to title case. Ex: joHn becomes John.
        /// </summary>
        public static string ToTitleCase(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source.ToLower());
        }
    }
}
