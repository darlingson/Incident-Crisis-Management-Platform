namespace Api.Tests;

using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Data.Repository;
using Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
public class ReportRepositoryTests
{
    private async Task<ApplicationDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new ApplicationDbContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    [Fact]
    public async Task AddAsync_ShouldSaveToDatabase()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);
        var report = new Report { Title = "Database Test" };

        await repository.AddAsync(report);
        await repository.SaveChangesAsync();

        var result = await context.Reports.CountAsync();
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveEvidenceFileAsync_CreatesDirectoryAndReturnsFileName()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportEvidenceRepository(context);
        var mockFile = CreateMockFile("evidence.png");

        var fileName = await repo.SaveEvidenceFileAsync(mockFile);

        fileName.Should().EndWith(".png");
        Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/evidence")).Should().BeTrue();

        if (Directory.Exists("wwwroot")) Directory.Delete("wwwroot", true);
    }
    [Fact]
    public async Task FindDuplicateAsync_ShouldReturnMatch_WhenWithinTimeWindow()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);
        var now = DateTime.UtcNow;

        var existingReport = new Report
        {
            Title = "Existing Fire",
            Type = "Fire",
            Location = "Sector 7",
            CreatedAt = now.AddMinutes(-10)
        };
        await context.Reports.AddAsync(existingReport);
        await context.SaveChangesAsync();

        var result = await repository.FindDuplicateAsync("Fire", "Sector 7", now);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Existing Fire");
    }

    [Fact]
    public async Task FindDuplicateAsync_ShouldReturnNull_WhenOutsideTimeWindow()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);
        var now = DateTime.UtcNow;

        var oldReport = new Report
        {
            Type = "Fire",
            Location = "Sector 7",
            CreatedAt = now.AddMinutes(-60)
        };
        await context.Reports.AddAsync(oldReport);
        await context.SaveChangesAsync();

        var result = await repository.FindDuplicateAsync("Fire", "Sector 7", now);

        result.Should().BeNull();
    }
    [Fact]
    public async Task AddAsync_ShouldAutoCategorize_WhenKeywordsArePresent()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);

        var report = new Report
        {
            Title = "Security Breach",
            Narrative = "An intruder was spotted near the server room.",
            Description = "Possible entry by intruder"
        };

        var result = await repository.AddAsync(report);
        await repository.SaveChangesAsync();

        var savedReport = await context.Reports
            .Include(r => r.ReportCategories)
            .FirstOrDefaultAsync(r => r.Id == result.Id);

        savedReport.Should().NotBeNull();
        savedReport!.ReportCategories.Should().Contain(rc => rc.CategoryId == 3);
    }

    [Fact]
    public async Task AddAsync_ShouldDefaultToOther_WhenNoKeywordsMatch()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);
        var report = new Report { Narrative = "Just a normal day, nothing specific happening." };

        var result = await repository.AddAsync(report);
        await repository.SaveChangesAsync();

        var savedReport = await context.Reports.Include(r => r.ReportCategories).FirstAsync();
        savedReport.ReportCategories.Should().Contain(rc => rc.CategoryId == 7);
    }
    [Fact]
    public async Task UpdateStatusAsync_ShouldFail_WhenResolvingWithoutImpact()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var report = new Report { Id = 1, Status = ReportStatus.UnderInvestigation, Impact = "" };
        context.Reports.Add(report);
        await context.SaveChangesAsync();

        var result = await repo.UpdateStatusAsync(1, ReportStatus.Resolved, 1, "Testing resolution");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Impact assessment is required");
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldCreateHistoryRecord_OnSuccess()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var report = new Report { Id = 1, Status = ReportStatus.Reported };
        context.Reports.Add(report);
        await context.SaveChangesAsync();

        await repo.UpdateStatusAsync(1, ReportStatus.Acknowledged, 99, "Acknowledging now");

        var history = await context.ReportStatusHistories.FirstOrDefaultAsync(h => h.ReportId == 1);
        history.Should().NotBeNull();
        history!.OldStatus.Should().Be(ReportStatus.Reported);
        history.NewStatus.Should().Be(ReportStatus.Acknowledged);
        history.ChangedBy.Should().Be(99);
    }
    [Fact]
    public async Task UpdateStatusAsync_ShouldAllowReopeningClosedIncident()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var oldDate = DateTime.UtcNow.AddDays(-5);
        var report = new Report { Id = 1, Status = ReportStatus.Closed, UpdatedAt = oldDate };
        context.Reports.Add(report);
        await context.SaveChangesAsync();

        var result = await repo.UpdateStatusAsync(1, ReportStatus.UnderInvestigation, 1, "New evidence found");

        result.Success.Should().BeTrue();
        var updatedReport = await context.Reports.FindAsync(1);
        updatedReport!.Status.Should().Be(ReportStatus.UnderInvestigation);
        updatedReport.UpdatedAt.Should().BeAfter(oldDate);
    }
    private IFormFile CreateMockFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(100);
        return fileMock.Object;
    }
}