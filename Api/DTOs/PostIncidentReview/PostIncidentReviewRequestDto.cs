namespace Api.DTOs.PostIncidentReview
{
    public class PostIncidentReviewRequestDto
    {
        public string RootCause { get; set; } = string.Empty;
        public string DetectionMethod { get; set; } = string.Empty;
        public string ReviewedBy { get; set; } = string.Empty;
        
        public List<RecoveryStepDto> RecoverySteps { get; set; } = new();
        public List<ContributingFactorDto> ContributingFactors { get; set; } = new();
        public List<PreventiveActionDto> PreventiveActions { get; set; } = new();
    }

    public class RecoveryStepDto
    {
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class ContributingFactorDto
    {
        public string Factor { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class PreventiveActionDto
    {
        public string ActionItem { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}