using System;

namespace Flavouru.Domain.Entities
{
    public class Media
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}

