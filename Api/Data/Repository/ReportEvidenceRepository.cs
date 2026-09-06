namespace Api.Data.Repository;

using Api.Data.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;
public class ReportEvidenceRepository : IReportEvidenceRepository
{
    private readonly ApplicationDbContext _context;
    public ReportEvidenceRepository(ApplicationDbContext context)
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
    public async Task UpdateAsync(ReportEvidence reportEvidence)
    {
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(ReportEvidence reportEvidence)
    {
        _context.ReportEvidences.Remove(reportEvidence);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}