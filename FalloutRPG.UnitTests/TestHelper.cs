using System.Runtime.CompilerServices;
using FalloutRPG.Data;
using FalloutRPG.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FalloutRPG.UnitTests
{
    public class TestHelper
    {
        /// <summary>
        /// Sets up the In-Memory RpgContext used for the unit tests.
        /// </summary>
        /// <param name="extName">Adds an extension to the test database name.
        /// When using xUnit Theory, add the test parameter value here. It is
        /// to ensure that no test uses the same database.</param>
        /// <param name="callerName">The method name of the unit test that called this. 
        /// (Just leave default)</param>
        /// <returns>The In-Memory RpgContext for testing.</returns>
        public static RpgContext SetupTestRpgContext(string extName = "",
            [CallerMemberName] string callerName = "")
        {
            return new RpgContext(new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: $"{callerName}_{extName}")
                .Options);
        }
    }
}