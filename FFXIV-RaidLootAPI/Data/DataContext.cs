using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
// List of tables for the DB
        public DbSet<Static> Statics { get; set; }

        public DbSet<Players> Players { get; set; }

        public DbSet<Gear> Gears { get; set; }

        public DbSet<Raid> Raids { get; set; }

        public DbSet<Users> User { get; set; }

        public DbSet<GearAcquisitionTimestamp> GearAcquisitionTimestamps {get;set;}
    }
}
