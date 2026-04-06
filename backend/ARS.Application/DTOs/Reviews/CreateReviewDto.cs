using ARS.Domain.Enums;

namespace ARS.Application.DTOs.Reviews;

public class CreateReviewDto
{
    public string AssignmentId { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public string ReviewedByUserId { get; set; } = string.Empty;
    public ReviewDecision Decision { get; set; }
    public string Comments { get; set; } = string.Empty;
}