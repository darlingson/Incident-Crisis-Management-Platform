namespace Api.Models;
public static class ReportStatusWorkflow
{
    private static readonly Dictionary<ReportStatus, List<ReportStatus>> ValidTransitions = 
        new Dictionary<ReportStatus, List<ReportStatus>>
    {
        { ReportStatus.Reported, new() { 
            ReportStatus.Acknowledged, 
            ReportStatus.UnderInvestigation, 
            ReportStatus.Closed
        }},
        { ReportStatus.Acknowledged, new() { 
            ReportStatus.UnderInvestigation, 
            ReportStatus.Resolved, 
            ReportStatus.Closed 
        }},
        { ReportStatus.UnderInvestigation, new() { 
            ReportStatus.Mitigation, 
            ReportStatus.Resolved, 
            ReportStatus.Closed 
        }},
        { ReportStatus.Mitigation, new() { 
            ReportStatus.UnderInvestigation,
            ReportStatus.Resolved 
        }},
        { ReportStatus.Resolved, new() { 
            ReportStatus.UnderInvestigation,
            ReportStatus.PostIncidentReview,
            ReportStatus.Closed 
        }},
        { ReportStatus.PostIncidentReview, new() { 
            ReportStatus.Closed 
        }},
        { ReportStatus.Closed, new() { 
            ReportStatus.UnderInvestigation
        }}
    };

    public static bool CanTransition(ReportStatus current, ReportStatus next)
    {
        if (current == next) return false;
        
        return ValidTransitions.TryGetValue(current, out var allowed) && allowed.Contains(next);
    }
}