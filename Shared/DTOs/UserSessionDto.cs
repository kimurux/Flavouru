using System;

namespace Flavouru.Shared.DTOs
{
    public class UserSessionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
    }
}