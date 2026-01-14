using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    public class PostIncidentReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report Report { get; set; } = null!;

        public string RootCause { get; set; } = string.Empty;
        public string DetectionMethod { get; set; } = string.Empty;
        
        public string ReviewedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public List<RecoveryStep> RecoverySteps { get; set; } = new();
        public List<ContributingFactor> ContributingFactors { get; set; } = new();
        public List<PreventiveAction> PreventiveActions { get; set; } = new();
    }

    public class RecoveryStep
    {
        [Key]
        public int Id { get; set; }
        public int PostIncidentReviewId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class ContributingFactor
    {
        [Key]
        public int Id { get; set; }
        public int PostIncidentReviewId { get; set; }
        public string Factor { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class PreventiveAction
    {
        [Key]
        public int Id { get; set; }
        public int PostIncidentReviewId { get; set; }
        public string ActionItem { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}