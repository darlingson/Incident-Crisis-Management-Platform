namespace Api.Data.Interfaces;

using Api.Models;

public interface IReportEvidenceRepository {
    Task<ReportEvidence?> GetByIdAsync(int id);
    Task<IEnumerable<ReportEvidence>> GetAllAsync();
    Task<ReportEvidence> AddAsync(ReportEvidence reportEvidence);
    Task UpdateAsync(ReportEvidence reportEvidence);
    Task DeleteAsync(ReportEvidence reportEvidence);
    Task<string> SaveEvidenceFileAsync(IFormFile file);
    Task SaveChangesAsync();
}
