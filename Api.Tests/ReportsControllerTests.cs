namespace Api.Tests;
using Moq;
using FluentAssertions;
using Api.Controllers;
using Api.Data.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

public class ReportsControllerTests
{
    private readonly Mock<IReportRepository> _mockRepo;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _mockRepo = new Mock<IReportRepository>();
        _controller = new ReportsController(_mockRepo.Object);
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
}