using System;
using System.Runtime.CompilerServices;
using FalloutRPG.Data;
using FalloutRPG.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FalloutRPG.Tests
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

        /// <summary>
        /// Build the config file for testing. Just a copy of the one in FalloutRPG.Main
        /// I would have just used that function but I didn't want to set it to public.
        /// The test config can be kept separate.
        /// </summary>
        /// <returns>Config</returns>
        public static IConfiguration BuildConfig() => new ConfigurationBuilder()
            .SetBasePath(System.IO.Path.GetFullPath(@"../../../"))
            .AddJsonFile("TestConfig.json")
            .Build();
    }
}