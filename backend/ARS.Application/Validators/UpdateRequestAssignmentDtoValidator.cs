using ARS.Application.DTOs.RequestAssignments;
using ARS.Domain.Enums;
using FluentValidation;

namespace ARS.Application.Validators { 

    public class UpdateRequestAssignmentDtoValidator : AbstractValidator<UpdateRequestAssignmentDto>
    {
        public UpdateRequestAssignmentDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames(typeof(AssignmentStatus)))}");

            RuleFor(x => x.NoProgressReason)
                .MaximumLength(500).WithMessage("NoProgressReason cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.NoProgressReason));
        }
    }
}