namespace Api.Data.Interfaces
{
    using Api.Models;
    using Api.DTOs.Reports;

    public interface IReportRepository {
        Task<Report?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Report>> GetAllWithDetailsAsync();
        Task<Report> AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Report report);
        Task SaveChangesAsync();
        Task<Report?> FindDuplicateAsync(string type, string location, DateTime currentTime);
        Task<Report?> GetEntityByIdAsync(int id);
        Task AddStatusHistoryAsync(ReportStatusHistory history);
    }
}