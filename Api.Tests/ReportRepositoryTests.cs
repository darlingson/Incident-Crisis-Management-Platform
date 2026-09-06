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
        var fileStorage = new Api.Services.FileStorageService();
        var service = new Api.Services.ReportEvidenceService(repo, fileStorage);
        var mockFile = CreateMockFile("evidence.png");

        var fileName = await service.SaveEvidenceFileAsync(mockFile);

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
    // Business logic moved to ReportService: categorization now tested via service
    [Fact]
    public async Task AddAsync_ShouldAutoCategorize_WhenKeywordsArePresent_ViaService()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var timeProvider = TimeProvider.System;
        var service = new Api.Services.ReportService(repository, evidenceService, categoryService, timeProvider);

        var dto = new Api.DTOs.Reports.CreateReportDto
        {
            Title = "Security Breach",
            Narrative = "An intruder was spotted near the server room.",
            Description = "Possible entry by intruder",
            Type = "security",
            Location = "Server Room",
            Impact = "high"
        };

        var result = await service.CreateReportAsync(dto);

        var savedReport = await context.Reports
            .Include(r => r.ReportCategories)
            .FirstOrDefaultAsync(r => r.Id == result.Id);

        savedReport.Should().NotBeNull();
        savedReport!.ReportCategories.Should().Contain(rc => rc.CategoryId == 3);
    }

    [Fact]
    public async Task AddAsync_ShouldDefaultToOther_WhenNoKeywordsMatch_ViaService()
    {
        var context = await GetDatabaseContext();
        var repository = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var timeProvider = TimeProvider.System;
        var service = new Api.Services.ReportService(repository, evidenceService, categoryService, timeProvider);

        var dto = new Api.DTOs.Reports.CreateReportDto
        {
            Title = "Generic",
            Narrative = "Just a normal day, nothing specific happening.",
            Description = "Just a normal day",
            Type = "other",
            Location = "Lobby",
            Impact = "low"
        };

        var result = await service.CreateReportAsync(dto);

        var savedReport = await context.Reports.Include(r => r.ReportCategories).FirstAsync();
        savedReport.ReportCategories.Should().Contain(rc => rc.CategoryId == 7);
    }
    [Fact]
    public async Task UpdateStatusAsync_ShouldFail_WhenResolvingWithoutImpact_ViaService()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var timeProvider = TimeProvider.System;
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider);
        var report = new Report { Id = 1, Status = ReportStatus.UnderInvestigation, Impact = "" };
        context.Reports.Add(report);
        await context.SaveChangesAsync();

        var result = await service.UpdateStatusAsync(1, ReportStatus.Resolved, 1, "Testing resolution");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Impact assessment is required");
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldCreateHistoryRecord_OnSuccess_ViaService()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var timeProvider = TimeProvider.System;
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider);
        var report = new Report { Id = 1, Status = ReportStatus.Reported };
        context.Reports.Add(report);
        await context.SaveChangesAsync();

        await service.UpdateStatusAsync(1, ReportStatus.Acknowledged, 99, "Acknowledging now");

        var history = await context.ReportStatusHistories.FirstOrDefaultAsync(h => h.ReportId == 1);
        history.Should().NotBeNull();
        history!.OldStatus.Should().Be(ReportStatus.Reported);
        history.NewStatus.Should().Be(ReportStatus.Acknowledged);
        history.ChangedBy.Should().Be(99);
    }
    [Fact]
    public async Task UpdateStatusAsync_ShouldAllowReopeningClosedIncident_ViaService()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var timeProvider = TimeProvider.System;
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider);
        var oldDate = DateTime.UtcNow.AddDays(-5);
        var report = new Report { Id = 1, Status = ReportStatus.Closed, UpdatedAt = oldDate };
        context.Reports.Add(report);
        await context.SaveChangesAsync();

        var result = await service.UpdateStatusAsync(1, ReportStatus.UnderInvestigation, 1, "New evidence found");

        result.Success.Should().BeTrue();
        var updatedReport = await context.Reports.FindAsync(1);
        updatedReport!.Status.Should().Be(ReportStatus.UnderInvestigation);
        updatedReport.UpdatedAt.Should().BeAfter(oldDate);
    }
    // New tests for extracted SRP services (Fix 1)
    [Theory]
    [InlineData("leak in plumbing", 2)]
    [InlineData("intruder theft", 3)]
    [InlineData("harassment payroll", 4)]
    [InlineData("slip hazard injury", 5)]
    [InlineData("audit policy violation", 6)]
    [InlineData("random text without keyword", 7)]
    public void CategorySuggestionService_ShouldReturnExpectedCategory(string narrative, int expectedCategoryId)
    {
        var service = new Api.Services.CategorySuggestionService();
        var result = service.GetSuggestedCategoryIds(narrative);
        result.Should().Contain(expectedCategoryId);
    }

    [Fact]
    public void CategorySuggestionService_ShouldReturnEmpty_ForNull()
    {
        var service = new Api.Services.CategorySuggestionService();
        var result = service.GetSuggestedCategoryIds(null);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FileStorageService_ShouldSaveFile_AndReturnName()
    {
        var storage = new Api.Services.FileStorageService();
        var mockFile = CreateMockFile("doc.pdf");
        var fileName = await storage.SaveFileAsync(mockFile, "evidence");
        fileName.Should().EndWith(".pdf");
        // cleanup
        if (Directory.Exists("wwwroot")) Directory.Delete("wwwroot", true);
    }

    [Fact]
    public async Task FileStorageService_ShouldThrow_ForEmptyFile()
    {
        var storage = new Api.Services.FileStorageService();
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);
        mockFile.Setup(f => f.FileName).Returns("empty.txt");
        Func<Task> act = async () => await storage.SaveFileAsync(mockFile.Object, "evidence");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    private class TestTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;
        public TestTimeProvider(DateTimeOffset now) => _now = now;
        public override DateTimeOffset GetUtcNow() => _now;
    }

    [Fact]
    public async Task ReportService_ShouldUseTimeProvider_ForTimestamps()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var fakeTime = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        var timeProvider = new TestTimeProvider(fakeTime);

        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider);
        var dto = new Api.DTOs.Reports.CreateReportDto
        {
            Title = "Time test",
            Narrative = "test",
            Description = "test",
            Type = "other",
            Location = "Lab",
            Impact = "low"
        };
        var report = await service.CreateReportAsync(dto);
        report.CreatedAt.Should().Be(fakeTime.UtcDateTime);
        report.UpdatedAt.Should().Be(fakeTime.UtcDateTime);
    }

    private IFormFile CreateMockFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(100);
        fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        return fileMock.Object;
    }
}