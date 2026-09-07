namespace Api.Controllers;

using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReportEvidenceController : ControllerBase
{
    private readonly IReportEvidenceService _reportEvidenceService;
    public ReportEvidenceController(IReportEvidenceService reportEvidenceService)
    {
        _reportEvidenceService = reportEvidenceService;
    }
}

