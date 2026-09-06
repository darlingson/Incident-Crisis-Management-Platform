using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IReportEvidenceService
    {
        Task<ReportEvidence?> GetByIdAsync(int id);
        Task<IEnumerable<ReportEvidence>> GetAllAsync();
        Task<ReportEvidence> AddAsync(ReportEvidence reportEvidence);
        Task UpdateAsync(ReportEvidence reportEvidence);
        Task DeleteAsync(ReportEvidence reportEvidence);
        Task<string> SaveEvidenceFileAsync(IFormFile file);
        Task SaveChangesAsync();
    }
}
