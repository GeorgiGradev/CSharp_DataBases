using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Configurations;
using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {
        }

        public FootballBettingContext(DbContextOptions options) 
            : base(options)
        {

        }     
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            TeamModelBuilder(modelBuilder);
            ColorModelBuilder(modelBuilder);
            TownModelBuilder(modelBuilder);
            CountryModelBuilder(modelBuilder);
            PlayerModelBuilder(modelBuilder);
            PositionModelBuilder(modelBuilder);
            PlayerStatisticModelBuilder(modelBuilder);
            GameModelBuilder(modelBuilder);
            BetModelBuilder(modelBuilder);
            UserModelBuilder(modelBuilder);
        }

        private static void UserModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
		
		        entity.Property(u => u.Username)
                .IsRequired(true);

                entity.Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired(true);

                entity.Property(u => u.Password)
                .HasMaxLength(256)
                .IsRequired(true);

            });
        }

        private static void BetModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(b => b.BetId);

                entity
                    .HasOne(b => b.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(b => b.GameId);

                entity
                    .HasOne(b => b.User)
                    .WithMany(u => u.Bets)
                    .HasForeignKey(b => b.UserId);
            });
        }

        private static void GameModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);

                entity.Property(g => g.Result)
                .IsRequired(true);

                entity
                    .HasOne(g => g.HomeTeam)
                    .WithMany(t => t.HomeGames)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict); ;

                entity
                    .HasOne(g => g.AwayTeam)
                    .WithMany(t => t.AwayGames)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void PlayerStatisticModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(ps => new
                {
                    ps.GameId,
                    ps.PlayerId
                });

                entity
                    .HasOne(ps => ps.Player)
                    .WithMany(p => p.PlayerStatistics)
                    .HasForeignKey(ps => ps.PlayerId);

                entity
                    .HasOne(ps => ps.Game)
                    .WithMany(g => g.PlayerStatistics)
                    .HasForeignKey(ps => ps.GameId);

                entity
                    .Property(ps => ps.MinutesPlayed)
                    .IsRequired(true);
            });
        }

        private static void PositionModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);

                entity.Property(p => p.Name)
                .IsRequired(true)
                .IsUnicode(true);
            });
        }

        private static void PlayerModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity.Property(p => p.Name)
                .IsRequired(true);

                entity
                    .HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId);

                entity
                    .HasOne(p => p.Position)
                    .WithMany(pos => pos.Players)
                    .HasForeignKey(p => p.PositionId);
            });
        }

        private static void CountryModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.CountryId);

                entity.Property(c => c.Name)
                .IsRequired(true);
            });
        }

        private static void TownModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);

                entity
                    .Property(t => t.Name);

                entity
                    .HasOne(t => t.Country)
                    .WithMany(c => c.Towns)
                    .HasForeignKey(t => t.CountryId);

            });
        }

        private static void ColorModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);

                entity.Property(c => c.Name)
                .IsRequired(true);
            });
        }

        private static void TeamModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);

                entity
                    .Property(t => t.Name)
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity
                    .Property(t => t.LogoUrl);

                entity
                    .Property(t => t.Initials);

                entity
                    .Property(t => t.Budget)
                    .HasColumnType("DECIMAL(18,2)");

                entity
                    .HasOne(t => t.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict); ;

                entity
                    .HasOne(t => t.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict); ;

                entity
                    .HasOne(team => team.Town)
                    .WithMany(town => town.Teams)
                    .HasForeignKey(team => team.TownId);
            });
        }
    }
}
