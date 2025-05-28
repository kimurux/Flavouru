using System;
using System.Threading.Tasks;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Interfaces
{
    public interface ISessionService
    {
        Task<UserSessionDto> CreateSessionAsync(Guid userId, string ipAddress, string deviceInfo);
        Task<bool> ValidateSessionAsync(string token);
        Task<bool> InvalidateSessionAsync(Guid sessionId);
        Task<UserSessionDto> RefreshSessionAsync(string token);
    }
}