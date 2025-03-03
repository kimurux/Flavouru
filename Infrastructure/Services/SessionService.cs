using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Flavouru.Application.Interfaces;
using Flavouru.Domain.Entities;
using Flavouru.Infrastructure.Data;
using Flavouru.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Flavouru.Infrastructure.Services
{
    public class SessionService : ISessionService
    {
        private readonly FlavouruDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public SessionService(
            FlavouruDbContext context,
            IConfiguration configuration,
            IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<UserSessionDto> CreateSessionAsync(Guid userId, string ipAddress, string deviceInfo)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            var expirationMinutes = _configuration.GetValue("Jwt:ExpirationInMinutes", 60);
            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                IpAddress = ipAddress,
                DeviceInfo = deviceInfo,
                IsActive = true,
                Token = GenerateJwtToken(user)
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserSessionDto>(session);
        }

        public async Task<bool> ValidateSessionAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            try
            {
                var principal = handler.ValidateToken(token, tokenValidationParameters, out _);

                // Explicitly use the first method
                var sessionId = principal.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.Id == Guid.Parse(sessionId) && s.IsActive);

                return session != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InvalidateSessionAsync(Guid sessionId)
        {
            var session = await _context.UserSessions.FindAsync(sessionId);
            if (session == null)
                return false;

            session.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserSessionDto> RefreshSessionAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var principal = handler.ValidateToken(token, tokenValidationParameters, out _);

            // Use explicit claim retrieval
            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var sessionId = principal.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;

            await InvalidateSessionAsync(Guid.Parse(sessionId));

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            return await CreateSessionAsync(user.Id, "Unknown", "Refreshed Session");
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("sid", Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue("Jwt:ExpirationInMinutes", 60)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}