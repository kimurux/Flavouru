using System;
using System.Collections.Generic;

namespace Flavouru.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}

