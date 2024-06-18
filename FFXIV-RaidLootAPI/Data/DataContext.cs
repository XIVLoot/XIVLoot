using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
// List of tables for the DB
        public DbSet<Static> Statics { get; set; }

        public DbSet<Players> Players { get; set; }

        public DbSet<Gear> Gears { get; set; }

        public DbSet<Raid> Raids { get; set; }

        public DbSet<Users> Users { get; set; }
    }
}
