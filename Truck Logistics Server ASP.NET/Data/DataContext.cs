using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Truck> Trucks { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>().HasData(
                new Language { Id = 1, Name = "Polish" },
                new Language { Id = 2, Name = "English" },
                new Language { Id = 3, Name = "German" }
                );

            modelBuilder.Entity<Users>().HasMany(u => u.AssignedTrucks).WithMany(t => t.AssignedUsers).UsingEntity(j => j.ToTable("UserTrucks"));


            //relacja jeden do wielu, klucz obcy assigneduserid, gdy usunie sie usera ustaw go na null
            modelBuilder.Entity<Job>().HasOne<Users>().WithMany(x => x.AssignedJobs).HasForeignKey(x => x.AssignedUserId).OnDelete(DeleteBehavior.SetNull);
        }


    }
}
