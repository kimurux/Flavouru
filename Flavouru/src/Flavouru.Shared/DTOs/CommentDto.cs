using System;

namespace Flavouru.Shared.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid RecipeId { get; set; }

        public CommentDto() { }

        public CommentDto(Guid id, string content, DateTime createdAt, Guid userId, string username, Guid recipeId)
        {
            Id = id;
            Content = content;
            CreatedAt = createdAt;
            UserId = userId;
            Username = username;
            RecipeId = recipeId;
        }
    }
}

