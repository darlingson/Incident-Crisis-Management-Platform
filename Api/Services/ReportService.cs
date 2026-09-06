using Api.Data.Interfaces;
using Api.DTOs.Reports;
using Api.Models;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportEvidenceService _reportEvidenceService;

        public ReportService(IReportRepository reportRepository, IReportEvidenceService reportEvidenceService)
        {
            _reportRepository = reportRepository;
            _reportEvidenceService = reportEvidenceService;
        }

        public async Task<IEnumerable<ReportResponseDto>> GetAllAsync()
        {
            return await _reportRepository.GetAllAsync();
        }

        public async Task<ReportResponseDto?> GetByIdAsync(int id)
        {
            return await _reportRepository.GetByIdAsync(id);
        }

        public async Task<Report> CreateReportAsync(CreateReportDto dto)
        {
            var report = new Report
            {
                Title = dto.Title,
                Type = dto.Type,
                Location = dto.Location,
                Narrative = dto.Narrative,
                Impact = dto.Impact,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = 1
            };

            // Business logic: auto-categorization (moved from repository)
            var suggestedCategoryIds = GetSuggestedCategoryIds(report.Narrative ?? report.Description);
            foreach (var categoryId in suggestedCategoryIds)
            {
                report.ReportCategories.Add(new ReportCategories
                {
                    CategoryId = categoryId
                });
            }

            var newReport = await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();

            // Handle evidence files via service (SOLID: file IO in service layer)
            if (dto.EvidenceFiles != null && dto.EvidenceFiles.Any())
            {
                foreach (var file in dto.EvidenceFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = await _reportEvidenceService.SaveEvidenceFileAsync(file);
                        report.ReportEvidences.Add(new ReportEvidence
                        {
                            ReportId = report.Id,
                            FilePath = fileName
                        });
                    }
                }
                await _reportRepository.SaveChangesAsync();
            }

            return newReport;
        }

        public async Task<DuplicateCheckResponse> CheckDuplicateAsync(DuplicateCheckDto dto)
        {
            var duplicate = await _reportRepository.FindDuplicateAsync(
                dto.Type,
                dto.Location,
                DateTime.UtcNow
            );

            if (duplicate == null)
            {
                return new DuplicateCheckResponse { IsDuplicate = false };
            }

            return new DuplicateCheckResponse
            {
                IsDuplicate = true,
                ExistingReportId = duplicate.Id,
                CreatedAt = duplicate.CreatedAt,
                Message = $"A similar {duplicate.Type} report already exists at this location.",
                ExistingReport = duplicate
            };
        }

        public async Task<ReportResponseDto?> GetReportStatusAsync(int id)
        {
            return await _reportRepository.GetByIdAsync(id);
        }

        public async Task<TransitionResult> UpdateStatusAsync(int id, ReportStatus newStatus, int changedBy, string? transitionNotes)
        {
            var report = await _reportRepository.GetEntityByIdAsync(id);
            if (report == null)
                return TransitionResult.Failure($"Report with ID {id} not found.");

            if (!ReportStatusWorkflow.CanTransition(report.Status, newStatus))
                return TransitionResult.Failure($"Illegal transition: You cannot move an incident from {report.Status} to {newStatus}.");

            if (newStatus == ReportStatus.Resolved && string.IsNullOrWhiteSpace(report.Impact))
                return TransitionResult.Failure("Resolution failed: Impact assessment is required before an incident can be marked as Resolved.");

            var history = new ReportStatusHistory
            {
                ReportId = report.Id,
                OldStatus = report.Status,
                NewStatus = newStatus,
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow,
                TransitionNotes = transitionNotes
            };

            report.Status = newStatus;
            report.UpdatedAt = DateTime.UtcNow;

            if (newStatus == ReportStatus.Resolved)
                report.ResolvedAt = DateTime.UtcNow;

            await _reportRepository.AddStatusHistoryAsync(history);
            await _reportRepository.SaveChangesAsync();

            return TransitionResult.Ok();
        }

        public async Task<TransitionResult> UpdateReportDetailsAsync(int id, ReportUpdateDto dto)
        {
            var report = await _reportRepository.GetEntityByIdAsync(id);
            if (report == null)
                return TransitionResult.Failure($"Report {id} not found.");

            report.Title = dto.Title;
            report.Narrative = dto.Narrative;
            report.Impact = dto.Impact;
            report.Location = dto.Location;
            report.Description = dto.Description;
            report.AssignedTo = dto.AssignedTo;
            report.Type = dto.Type;
            report.UpdatedAt = DateTime.UtcNow;

            await _reportRepository.SaveChangesAsync();
            return TransitionResult.Ok();
        }

        public async Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync()
        {
            return await _reportRepository.GetAssignableUsersAsync();
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
    }
}
