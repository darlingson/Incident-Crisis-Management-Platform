using Api.Models;

namespace Api.Data.Interfaces
{
    public interface IReportWriteRepository
    {
        Task<Report> AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Report report);
        Task AddStatusHistoryAsync(ReportStatusHistory history);
        Task SaveChangesAsync();
    }
}
