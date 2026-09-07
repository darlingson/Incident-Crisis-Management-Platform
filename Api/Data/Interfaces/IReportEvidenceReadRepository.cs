using Api.Models;

namespace Api.Data.Interfaces
{
    public interface IReportEvidenceReadRepository
    {
        Task<ReportEvidence?> GetByIdAsync(int id);
        Task<IEnumerable<ReportEvidence>> GetAllAsync();
    }
}
