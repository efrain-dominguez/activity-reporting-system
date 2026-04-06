using ARS.Application.DTOs.Reviews;
using ARS.Domain.Enums;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{

    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.AssignmentId)
                .NotEmpty().WithMessage("AssignmentId is required")
                .Must(BeValidObjectId).WithMessage("AssignmentId must be a valid MongoDB ObjectId");

            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("RequestId is required")
                .Must(BeValidObjectId).WithMessage("RequestId must be a valid MongoDB ObjectId");

            RuleFor(x => x.ReviewedByUserId)
                .NotEmpty().WithMessage("ReviewedByUserId is required")
                .Must(BeValidObjectId).WithMessage("ReviewedByUserId must be a valid MongoDB ObjectId");

            RuleFor(x => x.Decision)
                .IsInEnum().WithMessage($"Decision must be one of: {string.Join(", ", Enum.GetNames(typeof(ReviewDecision)))}");

            RuleFor(x => x.Comments)
                .NotEmpty().WithMessage("Comments is required")
                .MaximumLength(1000).WithMessage("Comments cannot exceed 1000 characters");
        }

        private bool BeValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }
    }
}