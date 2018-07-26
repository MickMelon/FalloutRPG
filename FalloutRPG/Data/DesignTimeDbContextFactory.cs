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
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.json")
                .Build();

            var builder = new DbContextOptionsBuilder<RpgContext>();

            var connectionString = configuration["sqlserver-connection-string"];

            builder.UseSqlServer(connectionString);

            return new RpgContext(builder.Options);
        }
    }
}
