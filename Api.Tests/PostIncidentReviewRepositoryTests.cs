namespace Api.Tests;

using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Data.Repository;
using Api.Models;
using Api.DTOs.PostIncidentReview;
using FluentAssertions;
using Xunit;

public class PostIncidentReviewRepositoryTests
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
    public async Task CreatePirAsync_ShouldSaveCorrectData_WhenValid()
    {
        // Arrange
        var context = await GetDatabaseContext();
        var repo = new PostIncidentReviewRepository(context);
        
        // Setup existing report
        context.Reports.Add(new Report { Id = 101, Title = "System Outage" });
        await context.SaveChangesAsync();

        var dto = new PostIncidentReviewRequestDto
        {
            RootCause = "Overloaded Load Balancer",
            DetectionMethod = "CloudWatch Alarm",
            ReviewedBy = "Senior SRE",
            RecoverySteps = new List<RecoveryStepDto> { new RecoveryStepDto { Description = "Restart Nodes", Order = 1 } }
        };

        // Act
        var result = await repo.CreatePirAsync(101, dto);

        // Assert
        result.Should().NotBeNull();
        result.ReportId.Should().Be(101);
        result.RecoverySteps.Should().HaveCount(1);
        
        var savedPir = await context.PostIncidentReviews.FirstOrDefaultAsync(p => p.ReportId == 101);
        savedPir!.RootCause.Should().Be("Overloaded Load Balancer");
    }

    [Fact]
    public async Task CreatePirAsync_ShouldThrowKeyNotFound_WhenReportMissing()
    {
        // Arrange
        var context = await GetDatabaseContext();
        var repo = new PostIncidentReviewRepository(context);
        var dto = new PostIncidentReviewRequestDto();

        // Act
        Func<Task> act = async () => await repo.CreatePirAsync(999, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdatePirAsync_ShouldClearOldSteps_AndAddNewOnes()
    {
        // Arrange
        var context = await GetDatabaseContext();
        var repo = new PostIncidentReviewRepository(context);
        
        var pir = new PostIncidentReview 
        { 
            ReportId = 200, 
            RecoverySteps = new List<RecoveryStep> { new RecoveryStep { Description = "Old Step" } } 
        };
        context.PostIncidentReviews.Add(pir);
        await context.SaveChangesAsync();

        var updateDto = new PostIncidentReviewRequestDto
        {
            RootCause = "Updated Cause",
            RecoverySteps = new List<RecoveryStepDto> 
            { 
                new RecoveryStepDto { Description = "Step A", Order = 1 },
                new RecoveryStepDto { Description = "Step B", Order = 2 }
            }
        };

        // Act
        await repo.UpdatePirAsync(200, updateDto);

        // Assert
        var updated = await context.PostIncidentReviews
            .Include(p => p.RecoverySteps)
            .FirstAsync(p => p.ReportId == 200);

        updated.RecoverySteps.Should().HaveCount(2);
        updated.RecoverySteps.Should().NotContain(s => s.Description == "Old Step");
    }
}