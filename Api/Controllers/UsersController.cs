using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return NotFound();

            var result = await _userService.GetProfileAsync(userId);
            if (!result.Succeeded)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPut("deactivate/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var result = await _userService.DeactivateUserAsync(userId);
            if (!result.Succeeded)
                return NotFound();

            return Ok(new { message = result.Message });
        }
    }
}