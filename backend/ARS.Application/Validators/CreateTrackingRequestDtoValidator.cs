using ARS.Application.DTOs.TrackingRequests;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{

    public class CreateTrackingRequestDtoValidator : AbstractValidator<CreateTrackingRequestDto>
    {
        public CreateTrackingRequestDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.GoalType)
                .NotEmpty().WithMessage("GoalType is required")
                .MaximumLength(100).WithMessage("GoalType cannot exceed 100 characters");

            RuleFor(x => x.TargetEntityIds)
                .NotEmpty().WithMessage("At least one target entity is required")
                .Must(BeValidObjectIdList).WithMessage("All target entity IDs must be valid MongoDB ObjectIds");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past");

            RuleFor(x => x.DueDate)
                .GreaterThan(x => x.StartDate).WithMessage("Due date must be after start date");

            RuleFor(x => x.Frequency)
                .MaximumLength(50).WithMessage("Frequency cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Frequency));
        }

        private bool BeValidObjectIdList(List<string> ids)
        {
            return ids.All(id => ObjectId.TryParse(id, out _));
        }
    }
}