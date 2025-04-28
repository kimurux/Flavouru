using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Flavouru.Domain.Entities;
using Flavouru.Application.Interfaces;
using Flavouru.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Flavouru.Shared.DTOs;
using AutoMapper;

namespace Flavouru.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly FlavouruDbContext _context;
        private readonly IMapper _mapper;

        public UserService(FlavouruDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerUserDto.Username || u.Email == registerUserDto.Email))
            {
                throw new InvalidOperationException("Пользователь с таким именем или email уже существует.");
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(registerUserDto.Password, salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> AuthenticateUserAsync(LoginUserDto loginUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUserDto.Username);

            if (user == null || !VerifyPassword(loginUserDto.Password, user.PasswordHash, user.Salt))
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null || !VerifyPassword(currentPassword, user.PasswordHash, user.Salt))
            {
                return false;
            }

            var newSalt = GenerateSalt();
            user.PasswordHash = HashPassword(newPassword, newSalt);
            user.Salt = newSalt;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LogoutUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var computedHash = HashPassword(password, storedSalt);
            return computedHash == storedHash;
        }
    }
}

