namespace Api.Models;

public class ReportEvidence
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public string FilePath { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}