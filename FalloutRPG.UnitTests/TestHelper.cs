using System.Runtime.CompilerServices;
using FalloutRPG.Data;
using FalloutRPG.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FalloutRPG.UnitTests
{
    public class TestHelper
    {
        public static RpgContext SetupTestRpgContext([CallerMemberName] string callerName = "")
            => new RpgContext(new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: callerName)
                .Options);
    }
}