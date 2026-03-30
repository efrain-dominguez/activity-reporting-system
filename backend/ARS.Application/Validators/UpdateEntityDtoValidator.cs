using ARS.Application.DTOs.Entities;
using FluentValidation;
using MongoDB.Bson;

namespace ARS.Application.Validators
{

    public class UpdateEntityDtoValidator : AbstractValidator<UpdateEntityDto>
    {
        public UpdateEntityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            // ← AGREGAR ESTA VALIDACIÓN
            RuleFor(x => x.ParentEntityId)
                .Must(BeValidObjectIdOrNull).WithMessage("ParentEntityId must be a valid MongoDB ObjectId or null");
        }

        private bool BeValidObjectIdOrNull(string? parentEntityId)
        {
            if (string.IsNullOrWhiteSpace(parentEntityId))
                return true;

            return ObjectId.TryParse(parentEntityId, out _);
        }
    }
}