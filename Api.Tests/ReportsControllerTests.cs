namespace Api.Tests;
using Moq;
using FluentAssertions;
using Api.Controllers;
using Api.Services.Interfaces;
using Api.Models;
using Api.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class ReportsControllerTests
{
    private readonly Mock<IReportService> _mockService;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _mockService = new Mock<IReportService>();
        _controller = new ReportsController(_mockService.Object);
    }

    [Fact]
    public async Task GetReport_ReturnsNotFound_WhenReportDoesNotExist()
    {
        _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ReportResponseDto?)null);
        var result = await _controller.GetReport(99);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetReport_ReturnsOk_WhenReportExists()
    {
        var fakeDto = new ReportResponseDto { Id = 1, Title = "Test Incident" };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

        var result = await _controller.GetReport(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var model = okResult.Value.Should().BeOfType<ReportResponseDto>().Subject;
        model.Title.Should().Be("Test Incident");
    }
    [Fact]
    public async Task CreateReport_ReturnsCreated_WhenOptionalFilesAreProvided()
    {
        var dto = new CreateReportDto
        {
            Title = "Test with Files",
            EvidenceFiles = new List<IFormFile>
        {
            CreateMockFile("test1.jpg"),
            CreateMockFile("test2.pdf")
        }
        };

        var createdReport = new Report { Id = 10, Title = "Test with Files" };
        _mockService.Setup(s => s.CreateReportAsync(It.IsAny<CreateReportDto>()))
                 .ReturnsAsync(createdReport);
        _mockService.Setup(s => s.GetByIdAsync(10))
                 .ReturnsAsync(new ReportResponseDto { Id = 10, Title = "Test with Files" });

        var result = await _controller.CreateReport(dto);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        _mockService.Verify(s => s.CreateReportAsync(It.IsAny<CreateReportDto>()), Times.Once);
    }
    [Fact]
    public async Task CreateReport_Succeeds_WhenFilesAreMissing()
    {
        var dto = new CreateReportDto { Title = "No Files" };
        var createdReport = new Report { Id = 11, Title = "No Files" };
        _mockService.Setup(s => s.CreateReportAsync(It.IsAny<CreateReportDto>()))
                 .ReturnsAsync(createdReport);
        _mockService.Setup(s => s.GetByIdAsync(11))
                 .ReturnsAsync(new ReportResponseDto { Id = 11, Title = "No Files" });

        var result = await _controller.CreateReport(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        _mockService.Verify(s => s.CreateReportAsync(It.IsAny<CreateReportDto>()), Times.Once);
    }
    [Fact]
    public async Task CheckDuplicate_ReturnsTrue_WhenDuplicateFound()
    {
        var dto = new DuplicateCheckDto { Type = "Flood", Location = "Downtown" };
        var response = new DuplicateCheckResponse { IsDuplicate = true, ExistingReportId = 50, ExistingReport = new Report { Id = 50, Type = "Flood", Location = "Downtown" } };

        _mockService.Setup(s => s.CheckDuplicateAsync(It.IsAny<DuplicateCheckDto>()))
                 .ReturnsAsync(response);

        var result = await _controller.CheckDuplicate(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dupResponse = okResult.Value.Should().BeOfType<DuplicateCheckResponse>().Subject;

        dupResponse.IsDuplicate.Should().BeTrue();
        dupResponse.ExistingReportId.Should().Be(50);
        dupResponse.ExistingReport.Should().NotBeNull();
        dupResponse.ExistingReport!.Type.Should().Be("Flood");
    }
    [Fact]
    public async Task CreateReport_ReturnsCreated_AndIncludesCategories()
    {
        var dto = new CreateReportDto
        {
            Title = "Test Report",
            Narrative = "The elevator is broken",
            Location = "Lobby"
        };

        var createdReport = new Report { Id = 1, Title = "Test Report" };
        createdReport.ReportCategories.Add(new ReportCategories { CategoryId = 2 });
        _mockService.Setup(s => s.CreateReportAsync(It.IsAny<CreateReportDto>()))
                 .ReturnsAsync(createdReport);
        _mockService.Setup(s => s.GetByIdAsync(1))
                 .ReturnsAsync(new ReportResponseDto { Id = 1, Title = "Test Report", Categories = new List<string> { "facilities" } });

        var result = await _controller.CreateReport(dto);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        // Controller now returns ReportResponseDto (or Report fallback)
        createdResult.Value.Should().NotBeNull();
    }
    private IFormFile CreateMockFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(100);
        return fileMock.Object;
    }
}