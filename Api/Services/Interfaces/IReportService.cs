using Api.DTOs.Reports;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IReportService : IReportQueryService, IReportCommandService, IReportWorkflowService
    {
    }
}
