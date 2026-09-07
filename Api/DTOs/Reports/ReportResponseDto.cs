namespace Api.DTOs.Reports
{
    public class ReportResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Narrative { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<string> EvidenceFiles { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public List<StatusHistoryDto> History { get; set; } = new();
    }

    public class StatusHistoryDto
    {
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? TransitionNotes { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
    }
}