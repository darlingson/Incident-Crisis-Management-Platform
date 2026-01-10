namespace Api.Tests;

using Moq;
using FluentAssertions;
using Api.Controllers;
using Api.Data.Interfaces;
using Api.Models;
using Api.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class ReportStatusWorkflowTests
{
    [Theory]
    [InlineData(ReportStatus.Reported, ReportStatus.Acknowledged, true)]
    [InlineData(ReportStatus.Reported, ReportStatus.Closed, true)]
    [InlineData(ReportStatus.Closed, ReportStatus.UnderInvestigation, true)]
    [InlineData(ReportStatus.Reported, ReportStatus.PostIncidentReview, false)]
    public void CanTransition_ShouldValidateExpectedPaths(ReportStatus current, ReportStatus next, bool expected)
    {
        var result = ReportStatusWorkflow.CanTransition(current, next);

        result.Should().Be(expected);
    }

    [Fact]
    public void CanTransition_ShouldReturnFalse_WhenStatesAreSame()
    {
        ReportStatusWorkflow.CanTransition(ReportStatus.Reported, ReportStatus.Reported).Should().BeFalse();
    }
}