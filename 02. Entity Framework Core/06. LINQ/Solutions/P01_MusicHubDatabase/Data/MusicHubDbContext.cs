namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongPerformer> SongsPerformers { get; set; }
        public DbSet<Writer> Writers { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SongPerformer>(entity =>
            {
                entity.HasKey(sp => new
                {
                    sp.SongId,
                    sp.PerformerId
                });

                entity
                    .HasOne(x => x.Song)
                    .WithMany(x => x.SongPerformers)
                    .HasForeignKey(x => x.SongId);

                entity
                    .HasOne(x => x.Performer)
                    .WithMany(x => x.PerformerSongs)
                    .HasForeignKey(x => x.PerformerId);
 
            });

            builder.Entity<Song>(entity =>
            {
                entity
                    .HasOne(x => x.Writer)
                    .WithMany(x => x.Songs)
                    .HasForeignKey(x => x.WriterId);
            });

            builder.Entity<Song>(entity =>
            {
                entity
                    .HasOne(x => x.Album)
                    .WithMany(x => x.Songs)
                    .HasForeignKey(x => x.AlbumId);
            });

            builder.Entity<Album>(entity =>
            {
                entity
                    .HasOne(x => x.Producer)
                    .WithMany(x => x.Albums)
                    .HasForeignKey(x => x.ProducerId);
            });
        }
    }
}
