namespace Api.DTOs.Reports;
using System.ComponentModel.DataAnnotations;
using Api.Models;
public class DuplicateCheckDto
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;
}

public class DuplicateCheckResponse
{
    public bool IsDuplicate { get; set; }
    public int? ExistingReportId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Message { get; set; }
    
    public Report? ExistingReport { get; set; }
}