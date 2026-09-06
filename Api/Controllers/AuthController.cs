using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SignUpAsync(model);
            if (!result.Succeeded)
                return BadRequest(new { Errors = result.Errors });

            return Ok(new
            {
                result.Message,
                result.Token,
                result.User
            });
        }

        [HttpPost("register-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUserAsync(model);
            if (!result.Succeeded)
            {
                if (result.Message != null && result.Message.Contains("does not exist"))
                    return BadRequest(new { message = result.Message });
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(new
            {
                result.Message,
                result.User
            });
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInDto model)
        {
            var result = await _authService.SignInAsync(model);
            if (!result.Succeeded)
                return Unauthorized(new { message = result.Message });

            return Ok(new
            {
                result.Token,
                result.RefreshToken,
                ExpiresIn = 60,
                result.User
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _authService.LogoutAsync(userId);
            }
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var result = await _authService.RefreshTokenAsync(model);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                result.Token,
                result.RefreshToken
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return NotFound();
            var result = await _authService.GetCurrentUserAsync(userId);
            if (!result.Succeeded)
                return NotFound();
            return Ok(result.User);
        }
    }
}