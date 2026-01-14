namespace Api.Data.Repository;

using Api.Data.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Api.DTOs.PostIncidentReview;

public class PostIncidentReviewRepository : IPostIncidentReviewRepository
{
    private readonly ApplicationDbContext _context;

    public PostIncidentReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PostIncidentReview?> GetPirByReportIdAsync(int reportId)
    {
        return await _context.PostIncidentReviews
            .Include(p => p.RecoverySteps)
            .Include(p => p.ContributingFactors)
            .Include(p => p.PreventiveActions)
            .FirstOrDefaultAsync(p => p.ReportId == reportId);
    }

    public async Task<PostIncidentReview> CreatePirAsync(int reportId, PostIncidentReviewRequestDto dto)
    {
        var reportExists = await _context.Reports.AnyAsync(r => r.Id == reportId);
        if (!reportExists) 
            throw new KeyNotFoundException($"Incident report with ID {reportId} not found.");

        var exists = await _context.PostIncidentReviews.AnyAsync(p => p.ReportId == reportId);
        if (exists) 
            throw new InvalidOperationException("A Post-Incident Review already exists for this incident.");

        var pir = new PostIncidentReview
        {
            ReportId = reportId,
            RootCause = dto.RootCause,
            DetectionMethod = dto.DetectionMethod,
            ReviewedBy = dto.ReviewedBy,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            RecoverySteps = dto.RecoverySteps.Select(s => new RecoveryStep { Description = s.Description, Order = s.Order }).ToList(),
            ContributingFactors = dto.ContributingFactors.Select(f => new ContributingFactor { Factor = f.Factor, Category = f.Category }).ToList(),
            PreventiveActions = dto.PreventiveActions.Select(a => new PreventiveAction { ActionItem = a.ActionItem, Owner = a.Owner, DueDate = a.DueDate, Status = "Pending" }).ToList()
        };

        _context.PostIncidentReviews.Add(pir);
        await _context.SaveChangesAsync();
        return pir;
    }

    public async Task<PostIncidentReview> UpdatePirAsync(int reportId, PostIncidentReviewRequestDto dto)
    {
        var pir = await _context.PostIncidentReviews
            .Include(p => p.RecoverySteps)
            .Include(p => p.ContributingFactors)
            .Include(p => p.PreventiveActions)
            .FirstOrDefaultAsync(p => p.ReportId == reportId);

        if (pir == null) 
            throw new KeyNotFoundException("Post-Incident Review not found.");

        pir.RootCause = dto.RootCause;
        pir.DetectionMethod = dto.DetectionMethod;
        pir.ReviewedBy = dto.ReviewedBy;
        pir.CompletedAt = DateTime.UtcNow;

        _context.RecoverySteps.RemoveRange(pir.RecoverySteps);
        _context.ContributingFactors.RemoveRange(pir.ContributingFactors);
        _context.PreventiveActions.RemoveRange(pir.PreventiveActions);

        pir.RecoverySteps = dto.RecoverySteps.Select(s => new RecoveryStep { Description = s.Description, Order = s.Order }).ToList();
        pir.ContributingFactors = dto.ContributingFactors.Select(f => new ContributingFactor { Factor = f.Factor, Category = f.Category }).ToList();
        pir.PreventiveActions = dto.PreventiveActions.Select(a => new PreventiveAction { ActionItem = a.ActionItem, Owner = a.Owner, DueDate = a.DueDate, Status = "Pending" }).ToList();

        await _context.SaveChangesAsync();
        return pir;
    }
}