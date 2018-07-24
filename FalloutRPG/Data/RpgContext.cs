using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FalloutRPG.Data
{
    public class RpgContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<NpcPreset> NpcPresets { get; set; }

        public RpgContext(DbContextOptions<RpgContext> options) : base(options)
        {
        }
    }
}
