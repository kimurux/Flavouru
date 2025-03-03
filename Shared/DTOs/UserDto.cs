using System;

namespace Flavouru.Shared.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto() { }

        public UserDto(Guid id, string username, string email, DateTime createdAt)
        {
            Id = id;
            Username = username;
            Email = email;
            CreatedAt = createdAt;
        }
    }
}