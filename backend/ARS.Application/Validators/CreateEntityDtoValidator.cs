using ARS.Application.DTOs.Entities;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{

    public class CreateEntityDtoValidator : AbstractValidator<CreateEntityDto>
    {
        public CreateEntityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(20).WithMessage("Code cannot exceed 20 characters")
                .Matches("^[A-Z0-9-]+$").WithMessage("Code can only contain uppercase letters, numbers, and hyphens");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

       
            RuleFor(x => x.ParentEntityId)
                .Must(id => string.IsNullOrWhiteSpace(id) || BeValidObjectId(id))
                .WithMessage("ParentEntityId must be a valid MongoDB ObjectId or null")
                .When(x => x.ParentEntityId != null);
        }

        private bool BeValidObjectIdOrNull(string? id)
        {
            // Si tiene valor, debe ser un ObjectId válido
            return ObjectId.TryParse(id, out _);
        }

        private bool BeValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }
    }
}