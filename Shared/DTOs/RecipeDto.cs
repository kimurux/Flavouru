using System;
using System.Collections.Generic;

namespace Flavouru.Shared.DTOs
{
    public class RecipeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
    }
    public class CreateRecipeDto
    {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Instructions { get; set; }
            public int PrepTime { get; set; }
            public int CookTime { get; set; }
            public int Servings { get; set; }
            public Guid UserId { get; set; }
            public List<Guid> CategoryIds { get; set; } = new List<Guid>();
            public List<Guid> TagIds { get; set; } = new List<Guid>();
        }

    public class UpdateRecipeDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public List<Guid> TagIds { get; set; }
    }
}

