using System;

namespace Flavouru.Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}

