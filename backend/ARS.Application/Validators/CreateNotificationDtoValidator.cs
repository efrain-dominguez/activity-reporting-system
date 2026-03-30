using FluentValidation;
using ARS.Application.DTOs.Notifications;
using MongoDB.Bson;

namespace ARS.Application.Validators
{

    public class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
    {
        public CreateNotificationDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required")
                .Must(BeValidObjectId).WithMessage("UserId must be a valid MongoDB ObjectId");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required")
                .MaximumLength(50).WithMessage("Type cannot exceed 50 characters");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters");

            RuleFor(x => x.RelatedEntityId)
                .Must(BeValidObjectIdOrNull).WithMessage("RelatedEntityId must be a valid MongoDB ObjectId or null")
                .When(x => !string.IsNullOrWhiteSpace(x.RelatedEntityId));
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