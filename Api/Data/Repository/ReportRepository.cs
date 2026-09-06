namespace Api.Data.Repository
{
    using Api.Data.Interfaces;
    using Api.Models;
    using Api.Data;
    using Microsoft.EntityFrameworkCore;
    public class ReportRepository : IReportRepository
    {
        private readonly IApplicationDbContext _context;

        public ReportRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Report>> GetAllWithDetailsAsync()
        {
            return await _context.Reports
                .AsNoTracking()
                .Include(r => r.ReportEvidences)
                .Include(r => r.ReportCategories).ThenInclude(rc => rc.Category)
                .Include(r => r.StatusHistories)
                .ToListAsync();
        }
        public async Task<Report?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Reports
                .AsNoTracking()
                .Include(r => r.ReportEvidences)
                .Include(r => r.ReportCategories).ThenInclude(rc => rc.Category)
                .Include(r => r.StatusHistories)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<Report> AddAsync(Report report)
        {
            var newReport = await _context.Reports.AddAsync(report);
            return newReport.Entity;
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Report report)
        {
            _context.Reports.Remove(report);
            return Task.CompletedTask;
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
        public async Task<Report?> GetEntityByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task AddStatusHistoryAsync(ReportStatusHistory history)
        {
            await _context.ReportStatusHistories.AddAsync(history);
        }


    }
}