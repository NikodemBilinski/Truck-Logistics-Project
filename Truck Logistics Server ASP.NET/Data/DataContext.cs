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

            // kazda faktura ma jednego klienta, kazdy ten klient moze miec wiele faktur, kazda faktura ma klucz obcy client.id
            modelBuilder.Entity<Invoice>().HasOne(x => x.Client).WithMany(x => x.Invoices).HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);

            // kazda faktura ma jedno zlecenie, kazde zlecenie moze miec wiele faktur, kazda faktura ma klucz obcy jobID, na usuniecie joba jobid = null
            modelBuilder.Entity<Invoice>().HasOne(x => x.Job).WithMany().HasForeignKey( x=> x.JobID).OnDelete(DeleteBehavior.SetNull);

            // kazde zlecenie ma jednego klienta, kazdy klient moze miec wiele zlecen, kazde zlecenie ma klucz obcy client.ID
            modelBuilder.Entity<Job>().HasOne(x => x.Client).WithMany(x=> x.Jobs).HasForeignKey(x=> x.ClientID).OnDelete(DeleteBehavior.Restrict);

            // ile liczb po przecinku w kwotach
            modelBuilder.Entity<Invoice>().Property(x => x.GrossAmount).HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>().Property(x => x.NetAmount).HasPrecision(18, 2);
        }


    }
}
