using Microsoft.EntityFrameworkCore;
using Stations.Models;

namespace Stations.Data
{

    public class StationsDbContext : DbContext
    {
        public StationsDbContext()
        {
        }

        public StationsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<CustomerCard> CustomerCards { get; set; }
        public DbSet<SeatingClass> SeatingClasses { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainSeat> TrainSeats { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Trip>(entity =>
            {
                entity
                    .HasOne(x => x.OriginStation)
                    .WithMany(x => x.TripsFrom)
                    .HasForeignKey(x => x.OriginStationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(x => x.DestinationStation)
                    .WithMany(x => x.TripsTo)
                    .HasForeignKey(x => x.DestinationStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Ticket>(entity =>
            {
                entity
                    .HasOne(x => x.CustomerCard)
                    .WithMany(x => x.BoughtTickets)
                    .HasForeignKey(x => x.CustomerCardId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Station>(entity =>
            {
                entity.HasAlternateKey(x => x.Name);
            });

            builder.Entity<Train>(entity =>
            {
                entity.HasAlternateKey(x => x.TrainNumber);
            });

            builder.Entity<SeatingClass>(entity =>
            {
                entity.HasAlternateKey(x=>x.Name);
                entity.HasAlternateKey(x=>x.Abbreviation);
            });

        }
    }
}