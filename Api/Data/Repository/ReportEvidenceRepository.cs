namespace Api.Data.Repository;

using Api.Data.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;
public class ReportEvidenceRepository : IReportEvidenceRepository
{
    private readonly IApplicationDbContext _context;
    public ReportEvidenceRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReportEvidence?> GetByIdAsync(int id)
    {
        return await _context.ReportEvidences.FindAsync(id);
    }
    public async Task<IEnumerable<ReportEvidence>> GetAllAsync()
    {
        return await _context.ReportEvidences.ToListAsync();
    }
    public async Task<ReportEvidence> AddAsync(ReportEvidence reportEvidence)
    {
        await _context.ReportEvidences.AddAsync(reportEvidence);
        return reportEvidence;
    }
    public Task UpdateAsync(ReportEvidence reportEvidence)
    {
        _context.ReportEvidences.Update(reportEvidence);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(ReportEvidence reportEvidence)
    {
        _context.ReportEvidences.Remove(reportEvidence);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}