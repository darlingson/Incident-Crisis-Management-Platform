namespace Api.Models
{ 
    public class Report {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public String Type { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public string Narrative { get; set; } = String.Empty;
        public string Impact { get; set; } = String.Empty;
        public string AssignedTo { get; set; } = String.Empty;
        public DateTime? ResolvedAt { get; set; }
        public string Description { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }

        public virtual ICollection<ReportEvidence> ReportEvidences { get; set; } = new List<ReportEvidence>();
    }
}