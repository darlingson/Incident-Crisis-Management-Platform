using Api.Models;
using Api.DTOs.Reports;

namespace Api.Mappers
{
    public interface IReportMapper
    {
        ReportResponseDto ToDto(Report report);
        IEnumerable<ReportResponseDto> ToDto(IEnumerable<Report> reports);
    }
}
