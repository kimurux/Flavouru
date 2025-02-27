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

        public RecipeDto()
        {
            Categories = new List<string>();
            Tags = new List<string>();
        }

        public RecipeDto(Guid id, string title, string description, string instructions, 
                         int prepTime, int cookTime, int servings, DateTime createdAt, 
                         DateTime? updatedAt, Guid userId, List<string> categories, List<string> tags)
        {
            Id = id;
            Title = title;
            Description = description;
            Instructions = instructions;
            PrepTime = prepTime;
            CookTime = cookTime;
            Servings = servings;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            UserId = userId;
            Categories = categories ?? new List<string>();
            Tags = tags ?? new List<string>();
        }
    }
}

