using ARS.Application.DTOs.Activities;
using ARS.Application.Validators;
using FluentAssertions;

namespace ARS.Tests.UnitTests.Validators;

public class CreateActivityValidatorTests
{
    private readonly CreateActivityDtoValidator _validator;

    public CreateActivityValidatorTests()
    {
        _validator = new CreateActivityDtoValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            AssignmentId = "507f1f77bcf86cd799439011",
            RequestId = "507f1f77bcf86cd799439012",
            Description = "Valid activity description",
            ActivityDate = DateTime.UtcNow.Date.AddDays(-1) // Yesterday
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldFail_WhenAssignmentIdIsEmpty()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            AssignmentId = "",
            RequestId = "507f1f77bcf86cd799439012",
            Description = "Valid description",
            ActivityDate = DateTime.UtcNow.Date.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AssignmentId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenRequestIdIsEmpty()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            AssignmentId = "507f1f77bcf86cd799439011",
            RequestId = "",
            Description = "Valid description",
            ActivityDate = DateTime.UtcNow.Date.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RequestId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionIsEmpty()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            AssignmentId = "507f1f77bcf86cd799439011",
            RequestId = "507f1f77bcf86cd799439012",
            Description = "",
            ActivityDate = DateTime.UtcNow.Date.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            AssignmentId = "507f1f77bcf86cd799439011",
            RequestId = "507f1f77bcf86cd799439012",
            Description = new string('x', 2001), // 2001 characters
            ActivityDate = DateTime.UtcNow.Date.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_ShouldFail_WhenActivityDateIsInFuture()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            AssignmentId = "507f1f77bcf86cd799439011",
            RequestId = "507f1f77bcf86cd799439012",
            Description = "Valid description",
            ActivityDate = DateTime.UtcNow.Date.AddDays(1) // Tomorrow
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ActivityDate");
    }
}