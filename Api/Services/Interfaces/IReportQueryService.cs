using Api.DTOs.Reports;

namespace Api.Services.Interfaces
{
    public interface IReportQueryService
    {
        Task<IEnumerable<ReportResponseDto>> GetAllAsync();
        Task<ReportResponseDto?> GetByIdAsync(int id);
        Task<DuplicateCheckResponse> CheckDuplicateAsync(DuplicateCheckDto dto);
        Task<ReportResponseDto?> GetReportStatusAsync(int id);
        Task<IEnumerable<UserSelectionDto>> GetAssignableUsersAsync();
    }
}
