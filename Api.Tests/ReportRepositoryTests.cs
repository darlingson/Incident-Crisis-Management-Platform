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
        var userRepo = new Api.Data.Repository.UserRepository(context);
        var service = new Api.Services.ReportService(repository, evidenceService, categoryService, timeProvider, userRepo);

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
        var userRepo = new Api.Data.Repository.UserRepository(context);
        var service = new Api.Services.ReportService(repository, evidenceService, categoryService, timeProvider, userRepo);

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
        var userRepo = new Api.Data.Repository.UserRepository(context);
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider, userRepo);
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
        var userRepo = new Api.Data.Repository.UserRepository(context);
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider, userRepo);
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
        var userRepo = new Api.Data.Repository.UserRepository(context);
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider, userRepo);
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
        var userRepo = new Api.Data.Repository.UserRepository(context);

        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, timeProvider, userRepo);
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

    // Fix 2: tests for SRP - mapper and user repo separation
    [Fact]
    public async Task ReportRepository_ShouldReturnEntities_WithDetails()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var report = new Report { Title = "Detail Test", Type = "other", Location = "Lab" };
        report.ReportEvidences.Add(new ReportEvidence { FilePath = "evidence1.png" });
        await repo.AddAsync(report);
        await repo.SaveChangesAsync();

        var all = await repo.GetAllWithDetailsAsync();
        all.Should().Contain(r => r.Title == "Detail Test" && r.ReportEvidences.Any(e => e.FilePath == "evidence1.png"));

        var byId = await repo.GetByIdWithDetailsAsync(report.Id);
        byId.Should().NotBeNull();
        byId!.ReportEvidences.Should().Contain(e => e.FilePath == "evidence1.png");
    }

    [Fact]
    public async Task ReportService_GetAll_ShouldMapToDto()
    {
        var context = await GetDatabaseContext();
        var repo = new ReportRepository(context);
        var evidenceRepo = new ReportEvidenceRepository(context);
        var fileStorage = new Api.Services.FileStorageService();
        var evidenceService = new Api.Services.ReportEvidenceService(evidenceRepo, fileStorage);
        var categoryService = new Api.Services.CategorySuggestionService();
        var userRepo = new Api.Data.Repository.UserRepository(context);
        var service = new Api.Services.ReportService(repo, evidenceService, categoryService, TimeProvider.System, userRepo);

        var dto = new Api.DTOs.Reports.CreateReportDto
        {
            Title = "Map Test",
            Narrative = "leak plumbing",
            Description = "leak",
            Type = "facilities",
            Location = "B1",
            Impact = "high"
        };
        await service.CreateReportAsync(dto);

        var allDtos = await service.GetAllAsync();
        allDtos.Should().Contain(d => d.Title == "Map Test" && d.Categories.Contains("facilities"));

        var singleDto = await service.GetByIdAsync(1);
        singleDto.Should().NotBeNull();
        singleDto!.EvidenceFiles.Should().BeOfType<List<string>>();
    }

    [Fact]
    public async Task UserRepository_ShouldReturnAssignableUsers()
    {
        var context = await GetDatabaseContext();
        var user = new Api.Models.ApplicationUser { Id = "u1", UserName = "test@test.com", Email = "test@test.com", FirstName = "Test", LastName = "User" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userRepo = new Api.Data.Repository.UserRepository(context);
        var result = await userRepo.GetAssignableUsersAsync();
        result.Should().Contain(u => u.Email == "test@test.com" && u.FullName == "Test User");
    }

    [Fact]
    public void ReportMapper_ShouldMapCorrectly()
    {
        var report = new Report
        {
            Id = 1,
            Title = "Mapper",
            Type = "safety",
            Status = ReportStatus.Reported,
            Location = "X",
            Narrative = "n",
            Impact = "low",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        report.ReportEvidences.Add(new ReportEvidence { FilePath = "a.png" });
        report.ReportCategories.Add(new ReportCategories { Category = new Category { Id = 5, Name = "safety" }, CategoryId = 5 });
        report.StatusHistories.Add(new ReportStatusHistory { OldStatus = ReportStatus.Reported, NewStatus = ReportStatus.Acknowledged, ChangedBy = 1, ChangedAt = DateTime.UtcNow });

        var dto = Api.Mappers.ReportMapper.ToDto(report);
        dto.Title.Should().Be("Mapper");
        dto.Status.Should().Be("Reported");
        dto.EvidenceFiles.Should().Contain("a.png");
        dto.Categories.Should().Contain("safety");
        dto.History.Should().HaveCount(1);
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