using ARS.Application.DTOs.RequestAssignments;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{

    public class CreateRequestAssignmentDtoValidator : AbstractValidator<CreateRequestAssignmentDto>
    {
        public CreateRequestAssignmentDtoValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("RequestId is required")
                .Must(BeValidObjectId).WithMessage("RequestId must be a valid MongoDB ObjectId");

            RuleFor(x => x.AssignedToEntityId)
                .NotEmpty().WithMessage("AssignedToEntityId is required")
                .Must(BeValidObjectId).WithMessage("AssignedToEntityId must be a valid MongoDB ObjectId");

            RuleFor(x => x.AssignedToUserId)
                .Must(BeValidObjectIdOrNull).WithMessage("AssignedToUserId must be a valid MongoDB ObjectId or null")
                .When(x => !string.IsNullOrWhiteSpace(x.AssignedToUserId));

            RuleFor(x => x.DelegatedFromUserId)
                .Must(BeValidObjectIdOrNull).WithMessage("DelegatedFromUserId must be a valid MongoDB ObjectId or null")
                .When(x => !string.IsNullOrWhiteSpace(x.DelegatedFromUserId));
        }

        private bool BeValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }

        private bool BeValidObjectIdOrNull(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return true;

            return ObjectId.TryParse(id, out _);
        }
    }
}