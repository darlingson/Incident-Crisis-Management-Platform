namespace Api.Controllers;

using Api.Data.Interfaces;
using Api.Models;
using Api.DTOs.ReportEvidences;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReportEvidenceController : ControllerBase
{
    private readonly IReportEvidenceRepository _reportEvidenceRepository;
    public ReportEvidenceController(IReportEvidenceRepository reportEvidenceRepository)
    {
        _reportEvidenceRepository = reportEvidenceRepository;
    }
}

