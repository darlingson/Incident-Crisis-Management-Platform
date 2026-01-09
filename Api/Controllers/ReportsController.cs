using Api.Models;
using Api.Data.Interfaces;
using Api.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportEvidenceRepository _reportEvidenceRepository;
        public ReportsController(IReportRepository reportRepository, IReportEvidenceRepository reportEvidenceRepository)
        {
            _reportRepository = reportRepository;
            _reportEvidenceRepository = reportEvidenceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            var reports = await _reportRepository.GetAllAsync();
            return Ok(reports);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null)
                return NotFound();
            return Ok(report);
        }
        [HttpPost]
        public async Task<ActionResult<Report>> CreateReport([FromForm] CreateReportDto dto)
        {
            var report = new Report
            {
                Title = dto.Title,
                Type = dto.Type,
                Status = "Open",
                Location = dto.Location,
                Narrative = dto.Narrative,
                Impact = dto.Impact,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = 1
            };
            var newReport = await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();

            if (dto.EvidenceFiles != null && dto.EvidenceFiles.Any())
            {
                foreach (var file in dto.EvidenceFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = await _reportEvidenceRepository.SaveEvidenceFileAsync(file);
                        report.ReportEvidences.Add(new ReportEvidence
                        {
                            ReportId = report.Id,
                            FilePath = fileName
                        });
                    }
                }
            }
            await _reportRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReport), new { id = newReport.Id }, newReport);
        }
        [HttpPost("check-duplicate")]
        public async Task<ActionResult<DuplicateCheckResponse>> CheckDuplicate(DuplicateCheckDto dto)
        {
            var duplicate = await _reportRepository.FindDuplicateAsync(
                dto.Type,
                dto.Location,
                DateTime.UtcNow
            );

            if (duplicate == null)
            {
                return Ok(new DuplicateCheckResponse { IsDuplicate = false });
            }

            return Ok(new DuplicateCheckResponse
            {
                IsDuplicate = true,
                ExistingReportId = duplicate.Id,
                CreatedAt = duplicate.CreatedAt,
                Message = $"A similar {duplicate.Type} report already exists at this location.",
                ExistingReport = duplicate
            });
        }
    }
}