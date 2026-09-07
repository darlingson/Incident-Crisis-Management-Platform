using Api.Models;
using Api.DTOs.Reports;

namespace Api.Mappers
{
    public static class ReportMapper
    {
        public static ReportResponseDto ToDto(Report report)
        {
            return new ReportResponseDto
            {
                Id = report.Id,
                Title = report.Title,
                Type = report.Type,
                Status = report.Status.ToString(),
                Location = report.Location,
                Narrative = report.Narrative,
                Impact = report.Impact,
                AssignedTo = report.AssignedTo,
                ResolvedAt = report.ResolvedAt,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt,
                EvidenceFiles = report.ReportEvidences.Select(e => e.FilePath).ToList(),
                Categories = report.ReportCategories.Select(rc => rc.Category.Name).ToList(),
                History = report.StatusHistories
                    .OrderByDescending(h => h.ChangedAt)
                    .Select(h => new StatusHistoryDto
                    {
                        OldStatus = h.OldStatus.ToString(),
                        NewStatus = h.NewStatus.ToString(),
                        TransitionNotes = h.TransitionNotes,
                        ChangedAt = h.ChangedAt,
                        ChangedBy = h.ChangedBy
                    }).ToList()
            };
        }

        public static IEnumerable<ReportResponseDto> ToDto(IEnumerable<Report> reports)
        {
            return reports.Select(ToDto);
        }
    }
}
