using System;
using System.Threading.Tasks;
using Flavouru.Application.Interfaces;
using Flavouru.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Flavouru.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
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
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginUserDto);
            if (user == null)
            {
                return Unauthorized("Неверное имя пользователя или пароль");
            }
            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Guid userId)
        {
            var result = await _userService.LogoutUserAsync(userId);
            if (!result)
            {
                return NotFound("Пользователь не найден");
            }
            return Ok();
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

