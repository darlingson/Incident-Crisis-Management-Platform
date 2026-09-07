using Api.DTOs;

namespace Api.Services.Interfaces
{
    public record AuthResult(
        bool Succeeded,
        string? Token = null,
        string? RefreshToken = null,
        AuthUserDto? User = null,
        IEnumerable<string>? Roles = null,
        IEnumerable<string>? Errors = null,
        string? Message = null
    );

    public interface IAuthService
    {
        Task<AuthResult> SignUpAsync(SignUpDto dto);
        Task<AuthResult> RegisterUserAsync(RegisterUserDto dto);
        Task<AuthResult> SignInAsync(SignInDto dto);
        Task<AuthResult> LogoutAsync(string userId);
        Task<AuthResult> RefreshTokenAsync(RefreshTokenDto dto);
        Task<AuthResult> GetCurrentUserAsync(string userId);
    }
}
