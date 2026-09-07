using Api.Models;

namespace Api.Data.Interfaces
{
    public interface IReportEvidenceWriteRepository
    {
        Task<ReportEvidence> AddAsync(ReportEvidence reportEvidence);
        Task UpdateAsync(ReportEvidence reportEvidence);
        Task DeleteAsync(ReportEvidence reportEvidence);
        Task SaveChangesAsync();
    }
}
