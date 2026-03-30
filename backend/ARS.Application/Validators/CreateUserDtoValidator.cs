using FluentValidation;
using ARS.Domain.Enums;
using ARS.Application.DTOs.Users;

namespace ARS.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.EntraObjectId)
            .NotEmpty().WithMessage("Entra Object ID is required")
            .MaximumLength(100).WithMessage("Entra Object ID cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(BeAValidRole).WithMessage($"Role must be one of: {string.Join(", ", Enum.GetNames(typeof(UserRole)))}");
    }

    private bool BeAValidRole(string role)
    {
        return Enum.TryParse<UserRole>(role, ignoreCase: true, out _);
    }
}