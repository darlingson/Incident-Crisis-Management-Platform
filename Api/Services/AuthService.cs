using Microsoft.AspNetCore.Identity;
using Api.Models;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly TimeProvider _timeProvider;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService,
            TimeProvider timeProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _timeProvider = timeProvider;
        }

        private static AuthUserDto ToDto(ApplicationUser user, IEnumerable<string> roles) => new()
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = roles
        };

        public async Task<AuthResult> SignUpAsync(SignUpDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new AuthResult(false, Errors: result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, "User");
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResult(
                true,
                Token: token,
                User: ToDto(user, roles),
                Roles: roles,
                Message: "User created successfully. Please check your email for confirmation."
            );
        }

        public async Task<AuthResult> RegisterUserAsync(RegisterUserDto dto)
        {
            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
            {
                return new AuthResult(false, Message: $"Role '{dto.Role}' does not exist");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new AuthResult(false, Errors: result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResult(
                true,
                User: ToDto(user, roles),
                Roles: roles,
                Message: $"User created successfully with {dto.Role} role"
            );
        }

        public async Task<AuthResult> SignInAsync(SignInDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthResult(false, Message: "Invalid email or password");
            }

            if (!user.IsActive)
            {
                return new AuthResult(false, Message: "Account is deactivated");
            }

            if (!user.EmailConfirmed && !dto.AllowUnconfirmedEmail)
            {
                return new AuthResult(false, Message: "Email not confirmed. Please check your email.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().UtcDateTime.AddDays(7);
            await _userManager.UpdateAsync(user);

            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResult(
                true,
                Token: token,
                RefreshToken: refreshToken,
                User: ToDto(user, roles),
                Roles: roles,
                Message: "Sign in successful"
            );
        }

        public async Task<AuthResult> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
            }
            return new AuthResult(true, Message: "Logged out successfully");
        }

        public async Task<AuthResult> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(dto.Token);
            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new AuthResult(false, Message: "Invalid token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                return new AuthResult(false, Message: "Invalid refresh token");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newToken = _jwtService.GenerateToken(user, roles);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = _timeProvider.GetUtcNow().UtcDateTime.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResult(true, Token: newToken, RefreshToken: newRefreshToken);
        }

        public async Task<AuthResult> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResult(false, Message: "User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new AuthResult(
                true,
                User: ToDto(user, roles),
                Roles: roles
            );
        }
    }
}
