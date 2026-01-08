namespace Api.Tests;
using Moq;
using FluentAssertions;
using Api.Controllers;
using Api.Data.Interfaces;
using Api.Models;
using Api.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class ReportsControllerTests
{
    private readonly Mock<IReportRepository> _mockRepo;
    private readonly ReportsController _controller;
    private readonly Mock<IReportEvidenceRepository> _mockEvidenceRepo;

    public ReportsControllerTests()
    {
        _mockRepo = new Mock<IReportRepository>();
        _mockEvidenceRepo = new Mock<IReportEvidenceRepository>();
        _controller = new ReportsController(_mockRepo.Object, _mockEvidenceRepo.Object);
    }

    [Fact]
    public async Task GetReport_ReturnsNotFound_WhenReportDoesNotExist()
    {
        _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Report)null);
        var result = await _controller.GetReport(99);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetReport_ReturnsOk_WhenReportExists()
    {
        var fakeReport = new Report { Id = 1, Title = "Test Incident" };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeReport);

        var result = await _controller.GetReport(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var model = okResult.Value.Should().BeOfType<Report>().Subject;
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

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Report>()))
                 .ReturnsAsync((Report r) => { r.Id = 10; return r; });

        var result = await _controller.CreateReport(dto);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
        _mockEvidenceRepo.Verify(e => e.SaveEvidenceFileAsync(It.IsAny<IFormFile>()), Times.Exactly(2));
    }
    [Fact]
    public async Task CreateReport_Succeeds_WhenFilesAreMissing()
    {
        var dto = new CreateReportDto { Title = "No Files" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Report>()))
                 .ReturnsAsync((Report r) => { r.Id = 11; return r; });

        var result = await _controller.CreateReport(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        _mockEvidenceRepo.Verify(e => e.SaveEvidenceFileAsync(It.IsAny<IFormFile>()), Times.Never);
    }
    private IFormFile CreateMockFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(100);
        return fileMock.Object;
    }
}