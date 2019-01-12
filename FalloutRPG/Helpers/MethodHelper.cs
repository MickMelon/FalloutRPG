using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FalloutRPG.Helpers
{
    public class MethodHelper
    {
        /// <summary>
        /// Gets the name of the method that calls this method. This is used
        /// for the unit tests when setting the in-memory database names.
        /// </summary>
        /// <param name="callerName">The name of the calling method.</param>
        /// <returns>The name of the calling method.</returns>
        public static string GetCurrentMethod([CallerMemberName] string callerName = "")
            => callerName;
    }
}