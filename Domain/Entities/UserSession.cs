using System;

namespace Flavouru.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
        public bool IsActive { get; set; }
    }
}