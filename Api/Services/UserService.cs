using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<UserResult> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserResult(false, Message: "User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return new UserResult(true, Data: new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.CreatedAt,
                Roles = roles
            });
        }

        public async Task<UserResult> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserResult(false, Message: "User not found");

            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            return new UserResult(true, Message: "User deactivated successfully");
        }
    }
}
