using System;

namespace Flavouru.Shared.DTOs
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateTagDto
    {
        public string Name { get; set; }
    }
}

