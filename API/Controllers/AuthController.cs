using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Flavouru.Application.Interfaces;
using Flavouru.Infrastructure.Services;
using Flavouru.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flavouru.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(registerUserDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginUserDto);
            if (user == null)
                return Unauthorized("Invalid credentials");

            // Create session with current request details
            var session = await _sessionService.CreateSessionAsync(
                user.Id,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString()
            );

            return Ok(new
            {
                User = user,
                Token = session.Token,
                ExpiresAt = session.ExpiresAt
            });
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            var isValid = await _sessionService.ValidateSessionAsync(token);
            return Ok(new { isValid });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string expiredToken)
        {
            try
            {
                var newSession = await _sessionService.RefreshSessionAsync(expiredToken);
                return Ok(new
                {
                    Token = newSession.Token,
                    ExpiresAt = newSession.ExpiresAt
                });
            }
            catch
            {
                return Unauthorized("Session could not be refreshed");
            }
        }

        [HttpPost("logout")]
        [Authorize] 
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out Guid parsedUserId))
            {
                var result = await _userService.LogoutUserAsync(parsedUserId);
                return result ? Ok() : BadRequest("Logout failed");
            }

            return BadRequest("Invalid user");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePasswordAsync(changePasswordDto.UserId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result)
            {
                return BadRequest("Не удалось изменить пароль");
            }
            return Ok();
        }
    }
}

