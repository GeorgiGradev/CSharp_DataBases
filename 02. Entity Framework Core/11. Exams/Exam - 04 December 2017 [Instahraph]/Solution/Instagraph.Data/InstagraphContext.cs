using Instagraph.Models;
using Microsoft.EntityFrameworkCore;

namespace Instagraph.Data
{
    public class InstagraphContext : DbContext
    {
        public InstagraphContext()
        {
        }

        public InstagraphContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFollower> UsersFollowers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserFollower>(entity =>
            {

                entity
                    .HasKey(x => new
                    {
                        x.UserId,
                        x.FollowerId
                    });

                entity.
                    HasOne(x => x.User)
                    .WithMany(x => x.Followers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(x => x.Follower)
                    .WithMany(x => x.UsersFollowing)
                    .HasForeignKey(x => x.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<User>(entity =>
            {
                entity
                .HasAlternateKey(x => x.Username);
            });

            builder
                .Entity<Post>(post =>
                {
                    post.HasOne(p => p.User)
                        .WithMany(u => u.Posts)
                        .HasForeignKey(p => p.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

                    post.HasOne(p => p.Picture)
                        .WithMany(pic => pic.Posts)
                        .HasForeignKey(p => p.PictureId)
                        .OnDelete(DeleteBehavior.Restrict);
                });
        }
    }
}