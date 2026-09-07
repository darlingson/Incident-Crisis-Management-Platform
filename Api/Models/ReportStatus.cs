namespace Api.Models
{
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
