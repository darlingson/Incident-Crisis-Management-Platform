using Api.DTOs.Reports;

namespace Api.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync();
    }
}
