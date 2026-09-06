using Api.DTOs;

namespace Api.Services.Interfaces
{
    public record UserResult(bool Succeeded, object? Data = null, string? Message = null);

    public interface IUserService
    {
        Task<IEnumerable<AuthUserDto>> GetAllUsersAsync();
        Task<UserResult> GetProfileAsync(string userId);
        Task<UserResult> DeactivateUserAsync(string userId);
    }
}
