using ARS.Application.DTOs.Activities;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{
    public class CreateActivityDtoValidator : AbstractValidator<CreateActivityDto>
    {
        public CreateActivityDtoValidator()
        {
            RuleFor(x => x.AssignmentId)
               .NotEmpty().WithMessage("AssignmentId is required")
               .Must(BeValidObjectId).WithMessage("AssignmentId must be a valid MongoDB ObjectId");

            RuleFor(x => x.RequestId)
               .NotEmpty().WithMessage("RequestId is required")
               .Must(BeValidObjectId).WithMessage("RequestId must be a valid MongoDB ObjectId");

            RuleFor(x => x.Description)
              .NotEmpty().WithMessage("Description is required")
              .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");


            RuleFor(x => x.ActivityDate)
              .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Activity date cannot be in the future");

        }

        private bool BeValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }
    }
}
