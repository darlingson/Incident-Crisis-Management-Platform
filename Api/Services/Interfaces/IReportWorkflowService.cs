using Api.DTOs.Reports;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IReportWorkflowService
    {
        Task<TransitionResult> UpdateStatusAsync(int id, ReportStatus newStatus, int changedBy, string? transitionNotes);
    }
}
