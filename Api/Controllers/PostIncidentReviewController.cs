namespace Api.Controllers;

using Api.Data.Interfaces;
using Api.Models;
using Api.DTOs.PostIncidentReview;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PostIncidentReviewController : ControllerBase
{
    private readonly IPostIncidentReviewRepository _postIncidentReviewRepository;

    public PostIncidentReviewController(IPostIncidentReviewRepository postIncidentReviewRepository)
    {
        _postIncidentReviewRepository = postIncidentReviewRepository;
    }

    [HttpGet("{reportId}")]
    public async Task<IActionResult> GetPIR(int reportId)
    {
        var pir = await _postIncidentReviewRepository.GetPirByReportIdAsync(reportId);
        if (pir == null) return NotFound(new { Message = "PIR not found." });
        return Ok(pir);
    }

    [HttpPost("{reportId}")]
    public async Task<IActionResult> CreatePIR(int reportId, [FromBody] PostIncidentReviewRequestDto dto)
    {
        try 
        {
            var pir = await _postIncidentReviewRepository.CreatePirAsync(reportId, dto);
            return CreatedAtAction(nameof(GetPIR), new { reportId = reportId }, pir);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }

    [HttpPut("{reportId}")]
    public async Task<IActionResult> UpdatePIR(int reportId, [FromBody] PostIncidentReviewRequestDto dto)
    {
        try
        {
            var pir = await _postIncidentReviewRepository.UpdatePirAsync(reportId, dto);
            return Ok(pir);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}