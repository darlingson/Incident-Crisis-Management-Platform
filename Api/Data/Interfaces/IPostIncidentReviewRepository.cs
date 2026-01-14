namespace Api.Data.Interfaces;

using Api.DTOs.PostIncidentReview;
using Api.Models;

public interface IPostIncidentReviewRepository
{
    Task<PostIncidentReview?> GetPirByReportIdAsync(int reportId);
    Task<PostIncidentReview> CreatePirAsync(int reportId, PostIncidentReviewRequestDto dto);
    Task<PostIncidentReview> UpdatePirAsync(int reportId, PostIncidentReviewRequestDto dto);
}