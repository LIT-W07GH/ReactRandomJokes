using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReactRandomJokes.Data
{
    public class JokesContext : DbContext
    {
        private string _connectionString;

        public JokesContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            modelBuilder.Entity<UserJokeLike>()
                .HasKey(qt => new { qt.UserId, qt.JokeId });

            modelBuilder.Entity<UserJokeLike>()
                .HasOne(qt => qt.User)
                .WithMany(q => q.UserJokeLikes)
                .HasForeignKey(q => q.UserId);

            modelBuilder.Entity<UserJokeLike>()
                .HasOne(qt => qt.Joke)
                .WithMany(t => t.UserJokeLikes)
                .HasForeignKey(q => q.JokeId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Joke> Jokes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserJokeLike> UserJokeLikes { get; set; }
    }
}