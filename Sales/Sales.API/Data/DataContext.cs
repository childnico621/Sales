using Microsoft.EntityFrameworkCore;
using Sales.Shared.Entities;

namespace Sales.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// al usar base de datos de sql azure se especifica el Tier y Max Size para que no elija una sobre escalada nivel 5
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasServiceTier("Basic");
            modelBuilder.HasDatabaseMaxSize("2 GB");
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex(s => new { s.CountryId, s.Name }).IsUnique();
            modelBuilder.Entity<City>().HasIndex(y => new { y.StateId, y.Name }).IsUnique();

            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        }
    }
}
