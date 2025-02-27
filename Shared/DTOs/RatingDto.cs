using System;

namespace Flavouru.Shared.DTOs
{
    public class RatingDto
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }

        public RatingDto() { }

        public RatingDto(Guid id, int value, DateTime createdAt, Guid userId, Guid recipeId)
        {
            Id = id;
            Value = value;
            CreatedAt = createdAt;
            UserId = userId;
            RecipeId = recipeId;
        }
    }
}

