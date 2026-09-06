using Api.DTOs.Reports;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReportResponseDto>> GetAllAsync();
        Task<ReportResponseDto?> GetByIdAsync(int id);
        Task<Report> CreateReportAsync(CreateReportDto dto);
        Task<DuplicateCheckResponse> CheckDuplicateAsync(DuplicateCheckDto dto);
        Task<ReportResponseDto?> GetReportStatusAsync(int id);
        Task<TransitionResult> UpdateStatusAsync(int id, ReportStatus newStatus, int changedBy, string? transitionNotes);
        Task<TransitionResult> UpdateReportDetailsAsync(int id, ReportUpdateDto dto);
        Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync();
    }
}
