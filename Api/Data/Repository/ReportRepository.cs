namespace Api.Data.Repository
{
    using Api.Data.Interfaces;
    using Api.Models;
    using Api.Data;
    using Microsoft.EntityFrameworkCore;
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports.ToListAsync();
        }
        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }
        public async Task<Report> AddAsync(Report report)
        {
            var NewReport = await _context.Reports.AddAsync(report);
            return NewReport.Entity;
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Report report)
        {
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Report report)
        {
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

        }
        public async Task<Report?> FindDuplicateAsync(string type, string location, DateTime currentTime)
        {
            var gracePeriod = currentTime.AddMinutes(-30);

            return await _context.Reports
                .AsNoTracking()
                .Where(r => r.Type == type && r.Location == location && r.CreatedAt >= gracePeriod)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}