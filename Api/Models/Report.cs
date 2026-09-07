namespace Api.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Narrative { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? ResolvedAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public ReportStatus Status { get; set; } = ReportStatus.Reported;

        public virtual ICollection<ReportEvidence> ReportEvidences { get; set; } = new List<ReportEvidence>();
        public virtual ICollection<ReportCategories> ReportCategories { get; set; } = new List<ReportCategories>();
        public virtual ICollection<ReportStatusHistory> StatusHistories { get; set; } = new List<ReportStatusHistory>();
    }
}