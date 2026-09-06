using Api.DTOs.Reports;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportResponseDto>>> GetReports()
        {
            var reports = await _reportService.GetAllAsync();
            return Ok(reports);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportResponseDto>> GetReport(int id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if (report == null)
                return NotFound();
            return Ok(report);
        }
        [HttpPost]
        public async Task<ActionResult<ReportResponseDto>> CreateReport([FromForm] CreateReportDto dto)
        {
            var newReport = await _reportService.CreateReportAsync(dto);
            var createdDto = await _reportService.GetByIdAsync(newReport.Id);
            return CreatedAtAction(nameof(GetReport), new { id = newReport.Id }, createdDto ?? (object)newReport);
        }
        [HttpPost("check-duplicate")]
        public async Task<ActionResult<DuplicateCheckResponse>> CheckDuplicate(DuplicateCheckDto dto)
        {
            var result = await _reportService.CheckDuplicateAsync(dto);
            return Ok(result);
        }
        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetReportStatus(int id)
        {
            var report = await _reportService.GetReportStatusAsync(id);

            if (report == null) return NotFound();

            return Ok(new
            {
                report.Id,
                report.Title,
                Status = report.Status
            });
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> TransitionStatus(int id, [FromBody] StatusTransitionDto dto)
        {
            var result = await _reportService.UpdateStatusAsync(id, dto.NewStatus, 1, dto.TransitionNotes);

            if (!result.Success)
            {
                return BadRequest(new { Error = result.Message });
            }

            return Ok(new
            {
                Message = "Status updated successfully",
                NewStatus = dto.NewStatus.ToString()
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] ReportUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reportService.UpdateReportDetailsAsync(id, dto);

            if (!result.Success)
            {
                return BadRequest(new { Error = result.Message });
            }

            return Ok(new { Message = "Report updated successfully" });
        }
        [HttpGet("assignable-users")]
        public async Task<ActionResult<IEnumerable<UserSelectionDto>>> GetAssignableUsers()
        {
            var users = await _reportService.GetAssignableUsersAsync();
            return Ok(users);
        }
    }
}