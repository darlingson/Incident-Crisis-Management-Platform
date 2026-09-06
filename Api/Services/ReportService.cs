using Api.Data.Interfaces;
using Api.DTOs.Reports;
using Api.Mappers;
using Api.Models;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportEvidenceService _reportEvidenceService;
        private readonly ICategorySuggestionService _categorySuggestionService;
        private readonly TimeProvider _timeProvider;
        private readonly IUserRepository _userRepository;

        public ReportService(
            IReportRepository reportRepository,
            IReportEvidenceService reportEvidenceService,
            ICategorySuggestionService categorySuggestionService,
            TimeProvider timeProvider,
            IUserRepository userRepository)
        {
            _reportRepository = reportRepository;
            _reportEvidenceService = reportEvidenceService;
            _categorySuggestionService = categorySuggestionService;
            _timeProvider = timeProvider;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ReportResponseDto>> GetAllAsync()
        {
            var reports = await _reportRepository.GetAllWithDetailsAsync();
            return ReportMapper.ToDto(reports);
        }

        public async Task<ReportResponseDto?> GetByIdAsync(int id)
        {
            var report = await _reportRepository.GetByIdWithDetailsAsync(id);
            return report == null ? null : ReportMapper.ToDto(report);
        }

        public async Task<Report> CreateReportAsync(CreateReportDto dto)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var report = new Report
            {
                Title = dto.Title,
                Type = dto.Type,
                Location = dto.Location,
                Narrative = dto.Narrative,
                Impact = dto.Impact,
                Description = dto.Description,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = 1
            };

            var suggestedCategoryIds = _categorySuggestionService.GetSuggestedCategoryIds(report.Narrative ?? report.Description);
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
                _timeProvider.GetUtcNow().UtcDateTime
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
            var report = await _reportRepository.GetByIdWithDetailsAsync(id);
            return report == null ? null : ReportMapper.ToDto(report);
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

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var history = new ReportStatusHistory
            {
                ReportId = report.Id,
                OldStatus = report.Status,
                NewStatus = newStatus,
                ChangedBy = changedBy,
                ChangedAt = now,
                TransitionNotes = transitionNotes
            };

            report.Status = newStatus;
            report.UpdatedAt = now;

            if (newStatus == ReportStatus.Resolved)
                report.ResolvedAt = now;

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
            report.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;

            await _reportRepository.SaveChangesAsync();
            return TransitionResult.Ok();
        }

        public async Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync()
        {
            return await _userRepository.GetAssignableUsersAsync();
        }


    }
}
