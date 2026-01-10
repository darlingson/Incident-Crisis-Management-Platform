namespace Api.DTOs.Reports;
using Api.Models;
public class StatusTransitionDto
{
    public ReportStatus NewStatus { get; set; }
    public string? TransitionNotes { get; set; } = string.Empty;
}
public class TransitionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static TransitionResult Failure(string message) => new() { Success = false, Message = message };
    public static TransitionResult Ok() => new() { Success = true, Message = "Success" };
}