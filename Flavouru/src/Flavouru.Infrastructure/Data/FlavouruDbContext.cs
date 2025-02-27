using Flavouru.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flavouru.Infrastructure.Data
{
    public class FlavouruDbContext : DbContext
    {
        public FlavouruDbContext(DbContextOptions<FlavouruDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Media> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any additional configuration here
        }
    }
}

