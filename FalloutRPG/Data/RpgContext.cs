﻿using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FalloutRPG.Data
{
    public class RpgContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Item> Items { get; set; }

        public RpgContext(DbContextOptions<RpgContext> options) : base(options)
        {
        }
    }
}
