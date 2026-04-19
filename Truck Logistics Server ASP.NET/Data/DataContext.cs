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
    }
}
