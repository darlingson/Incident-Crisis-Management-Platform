namespace Api.Data.Interfaces
{
    using Api.Models;

    public interface IReportRepository {
        Task<Report?> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report> AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Report report);
        Task SaveChangesAsync();
    }
}