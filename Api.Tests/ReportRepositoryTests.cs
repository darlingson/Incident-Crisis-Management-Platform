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
    private IFormFile CreateMockFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(100);
        return fileMock.Object;
    }
}