namespace Api.Data.Repository
{
    using Api.Data.Interfaces;
    using Api.Models;
    using Api.Data;
    using Api.DTOs.Reports;
    using Microsoft.EntityFrameworkCore;
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ReportResponseDto>> GetAllAsync()
        {
            return await _context.Reports
                .AsNoTracking()
                .Select(r => new ReportResponseDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Type = r.Type,
                    Status = r.Status.ToString(),
                    Location = r.Location,
                    Narrative = r.Narrative,
                    Impact = r.Impact,
                    AssignedTo = r.AssignedTo,
                    ResolvedAt = r.ResolvedAt,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    EvidenceFiles = r.ReportEvidences.Select(e => e.FilePath).ToList(),
                    Categories = r.ReportCategories.Select(rc => rc.Category.Name).ToList(),
                    History = r.StatusHistories
                        .OrderByDescending(h => h.ChangedAt)
                        .Select(h => new StatusHistoryDto
                        {
                            OldStatus = h.OldStatus.ToString(),
                            NewStatus = h.NewStatus.ToString(),
                            TransitionNotes = h.TransitionNotes,
                            ChangedAt = h.ChangedAt,
                            ChangedBy = h.ChangedBy
                        }).ToList()
                })
                .ToListAsync();
        }
        public async Task<ReportResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .AsNoTracking()
                .Select(r => new ReportResponseDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Type = r.Type,
                    Status = r.Status.ToString(),
                    Location = r.Location,
                    Narrative = r.Narrative,
                    Impact = r.Impact,
                    AssignedTo = r.AssignedTo,
                    ResolvedAt = r.ResolvedAt,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    EvidenceFiles = r.ReportEvidences.Select(e => e.FilePath).ToList(),
                    Categories = r.ReportCategories.Select(rc => rc.Category.Name).ToList(),
                    History = r.StatusHistories
                        .OrderByDescending(h => h.ChangedAt)
                        .Select(h => new StatusHistoryDto
                        {
                            OldStatus = h.OldStatus.ToString(),
                            NewStatus = h.NewStatus.ToString(),
                            TransitionNotes = h.TransitionNotes,
                            ChangedAt = h.ChangedAt,
                            ChangedBy = h.ChangedBy
                        }).ToList()
                })
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
        public async Task<Report?> GetEntityByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task AddStatusHistoryAsync(ReportStatusHistory history)
        {
            await _context.ReportStatusHistories.AddAsync(history);
        }

        public async Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(u => new UserSelectionDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email
                })
                .ToListAsync();
        }
    }
}