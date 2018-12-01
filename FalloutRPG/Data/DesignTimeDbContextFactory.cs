using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FalloutRPG.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RpgContext>
    {
        public RpgContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<RpgContext>();

            builder.UseSqlite("Filename=CharacterDB.db");

            return new RpgContext(builder.Options);
        }
    }
}
