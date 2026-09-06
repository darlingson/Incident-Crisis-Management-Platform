using Api.DTOs.Reports;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IReportWorkflowService
    {
        Task<TransitionResult> UpdateStatusAsync(int id, ReportStatus newStatus, string changedBy, string? transitionNotes);
    }
}
