using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;

namespace P03_SalesDatabase.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext()
        {
        }

        public SalesDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.StringConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name)
                .IsUnicode(true);

                entity.Property(p => p.Description)
                .HasDefaultValue("No description");

            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.Name)
                .IsUnicode(true);

                entity.Property(c => c.Email)
                .IsUnicode(false);


                entity.Property(p => p.CreditCardNumber)
                .IsUnicode(false);

            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(s => s.Name)
                .IsUnicode(true);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(s => s.Date)
                .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
