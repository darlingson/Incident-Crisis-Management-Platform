using Api.DTOs.Reports;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IReportCommandService
    {
        Task<Report> CreateReportAsync(CreateReportDto dto);
        Task<TransitionResult> UpdateReportDetailsAsync(int id, ReportUpdateDto dto);
    }
}
