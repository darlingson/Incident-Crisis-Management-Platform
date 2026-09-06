using Api.Models;

namespace Api.Data.Interfaces
{
    public interface IReportReadRepository
    {
        Task<Report?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Report>> GetAllWithDetailsAsync();
        Task<Report?> GetEntityByIdAsync(int id);
        Task<Report?> FindDuplicateAsync(string type, string location, DateTime currentTime);
    }
}
