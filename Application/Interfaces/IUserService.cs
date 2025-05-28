using System;
using System.Threading.Tasks;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<UserDto> AuthenticateUserAsync(LoginUserDto loginUserDto);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> LogoutUserAsync(Guid userId);
    }
}