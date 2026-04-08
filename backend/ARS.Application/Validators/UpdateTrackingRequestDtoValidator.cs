using ARS.Application.DTOs.TrackingRequests;
using ARS.Domain.Enums;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{
    public class UpdateTrackingRequestDtoValidator : AbstractValidator<UpdateTrackingRequestDto>
    {

        public UpdateTrackingRequestDtoValidator()
        {

             RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.GoalType)
                .NotEmpty().WithMessage("Goal type is required")
                .MaximumLength(100).WithMessage("Goal type cannot exceed 100 characters");

            RuleFor(x => x.TargetEntityIds)
                .NotEmpty().WithMessage("At least one target entity is required")
                .Must(ids => ids.All(id => ObjectId.TryParse(id, out _)))
                .WithMessage("All target entity IDs must be valid MongoDB ObjectIds");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .GreaterThan(x => x.StartDate).WithMessage("Due date must be after start date");

            RuleFor(x => x.Frequency)
                .MaximumLength(50).WithMessage("Frequency cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Frequency));

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames(typeof(RequestStatus)))}");
        }

        private bool BeValidObjectIdList(List<string> ids)
        {
            return ids.All(id => ObjectId.TryParse(id, out _));
        }
    }
}
