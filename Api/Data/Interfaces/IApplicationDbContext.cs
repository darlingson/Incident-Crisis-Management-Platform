using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Report> Reports { get; }
        DbSet<ReportEvidence> ReportEvidences { get; }
        DbSet<Category> Categories { get; }
        DbSet<ReportCategories> ReportCategories { get; }
        DbSet<ReportStatusHistory> ReportStatusHistories { get; }
        DbSet<ApplicationUser> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
