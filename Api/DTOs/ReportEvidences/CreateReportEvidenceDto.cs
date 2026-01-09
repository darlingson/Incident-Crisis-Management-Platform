namespace Api.DTOs.ReportEvidences;

public class CreateReportEvidenceDto
{
    public int ReportId { get; set; }
    public IFormFile File { get; set; } = null!;
}
