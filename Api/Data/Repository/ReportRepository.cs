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
            return await _context.Reports
            .Include(r => r.ReportCategories)
            .ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<Report> AddAsync(Report report)
        {
            var suggestedCategoryIds = GetSuggestedCategoryIds(report.Narrative ?? report.Description);
            foreach (var categoryId in suggestedCategoryIds)
            {
                report.ReportCategories.Add(new ReportCategories
                {
                    CategoryId = categoryId
                });
            }
            var NewReport = await _context.Reports.AddAsync(report);
            return NewReport.Entity;
        }
        private List<int> GetSuggestedCategoryIds(string content)
        {
            var suggestions = new List<int>();
            if (string.IsNullOrWhiteSpace(content)) return suggestions;

            var text = content.ToLower();

            if (text.Contains("leak") || text.Contains("plumbing") || text.Contains("elevator"))
                suggestions.Add(2);

            if (text.Contains("theft") || text.Contains("intruder") || text.Contains("unauthorized"))
                suggestions.Add(3);

            if (text.Contains("harassment") || text.Contains("bullying") || text.Contains("payroll"))
                suggestions.Add(4);

            if (text.Contains("slip") || text.Contains("fall") || text.Contains("hazard") || text.Contains("injury"))
                suggestions.Add(5);

            if (text.Contains("audit") || text.Contains("policy") || text.Contains("violation"))
                suggestions.Add(6);

            if (!suggestions.Any())
                suggestions.Add(7);

            return suggestions.Distinct().ToList();
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
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
        public async Task UpdateStatusAsync(int id, ReportStatus newStatus)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                report.Status = newStatus;
                report.UpdatedAt = DateTime.UtcNow;

                if (newStatus == ReportStatus.Resolved)
                {
                    report.ResolvedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}