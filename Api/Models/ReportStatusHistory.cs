namespace Api.Models
{
    public class ReportStatusHistory
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        
        public ReportStatus OldStatus { get; set; }
        public ReportStatus NewStatus { get; set; }
        
        public int ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? TransitionNotes { get; set; }

        public virtual Report Report { get; set; } = null!;
    }
}