namespace Api.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public String Type { get; set; } = String.Empty;
        // public string Status { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public string Narrative { get; set; } = String.Empty;
        public string Impact { get; set; } = String.Empty;
        public string AssignedTo { get; set; } = String.Empty;
        public DateTime? ResolvedAt { get; set; }
        public string Description { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Reported;

        public virtual ICollection<ReportEvidence> ReportEvidences { get; set; } = new List<ReportEvidence>();
        public virtual ICollection<ReportCategories> ReportCategories { get; set; } = new List<ReportCategories>();
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
    public class ReportCategories
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int CategoryId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Report Report { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
    public enum ReportStatus
    {
        Reported = 0,
        Acknowledged = 1,
        UnderInvestigation = 2,
        Mitigation = 3,
        Resolved = 4,
        PostIncidentReview = 5,
        Closed = 6
    }
}