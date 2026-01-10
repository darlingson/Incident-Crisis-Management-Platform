namespace Api.Data.Interfaces
{
    using Api.Models;
    using Api.DTOs.Reports;

    public interface IReportRepository {
        Task<Report?> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report> AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Report report);
        Task SaveChangesAsync();
        Task<Report?> FindDuplicateAsync(string type, string location, DateTime currentTime);
        Task<TransitionResult> UpdateStatusAsync(int id, ReportStatus newStatus, int changedBy, string? transitionNotes);
    }
}