using Flavouru.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flavouru.Infrastructure.Data
{
    public class FlavouruDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public FlavouruDbContext(DbContextOptions<FlavouruDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Categories)
                .WithMany(c => c.Recipes)
                .UsingEntity(j => j.ToTable("RecipeCategories"));

            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Tags)
                .WithMany(t => t.Recipes)
                .UsingEntity(j => j.ToTable("RecipeTags"));
        }
    }
}