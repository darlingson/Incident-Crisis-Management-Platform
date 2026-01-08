namespace Api.Tests;

using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Data.Repository;
using Api.Models;
using FluentAssertions;

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
}