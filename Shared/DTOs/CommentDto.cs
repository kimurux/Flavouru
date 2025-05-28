using System;

namespace Flavouru.Shared.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public Guid RecipeId { get; set; }
    }

    public class CreateCommentDto
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Content { get; set; }
    }
}

