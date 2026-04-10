using ARS.Domain.Entities;
using ARS.Infrastructure.Repositories;
using ARS.Tests.Helpers;
using FluentAssertions;

namespace ARS.Tests.UnitTests.Repositories;

public class ActivityRepositoryTests : TestBase
{
    private readonly ActivityRepository _repository;

    public ActivityRepositoryTests()
    {
        _repository = new ActivityRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateActivity()
    {
        // Arrange
        var activity = new Activity
        {
            AssignmentId = "507f1f77bcf86cd799439011",
            RequestId = "507f1f77bcf86cd799439012",
            Description = "Test activity",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439013",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(activity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Description.Should().Be("Test activity");
        result.AssignmentId.Should().Be("507f1f77bcf86cd799439011");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnActivity_WhenActivityExists()
    {
        // Arrange
        var activity = new Activity
        {
            AssignmentId = "507f1f77bcf86cd799439014",
            RequestId = "507f1f77bcf86cd799439015",
            Description = "Existing activity",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439016",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _repository.CreateAsync(activity);

        // Act
        var result = await _repository.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(created.Id);
        result.Description.Should().Be("Existing activity");
    }

    [Fact]
    public async Task GetBySubmittedByUserIdAsync_ShouldReturnUserActivities()
    {
        // Arrange
        var userId = "507f1f77bcf86cd799439017";

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = "507f1f77bcf86cd799439018",
            RequestId = "507f1f77bcf86cd799439019",
            Description = "User activity 1",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = userId,
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = "507f1f77bcf86cd79943901a",
            RequestId = "507f1f77bcf86cd79943901b",
            Description = "User activity 2",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = userId,
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = "507f1f77bcf86cd79943901c",
            RequestId = "507f1f77bcf86cd79943901d",
            Description = "Other user activity",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd79943901e",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Act
        var result = await _repository.GetBySubmittedByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.SubmittedByUserId == userId);
    }

    [Fact]
    public async Task GetByAssignmentIdAsync_ShouldReturnAssignmentActivities()
    {
        // Arrange
        var assignmentId = "507f1f77bcf86cd79943901f";

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = assignmentId,
            RequestId = "507f1f77bcf86cd799439020",
            Description = "Assignment activity 1",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439021",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = assignmentId,
            RequestId = "507f1f77bcf86cd799439022",
            Description = "Assignment activity 2",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439023",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = "507f1f77bcf86cd799439024",
            RequestId = "507f1f77bcf86cd799439025",
            Description = "Other assignment activity",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439026",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Act
        var result = await _repository.GetByAssignmentIdAsync(assignmentId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.AssignmentId == assignmentId);
    }

    [Fact]
    public async Task GetByRequestIdAsync_ShouldReturnRequestActivities()
    {
        // Arrange
        var requestId = "507f1f77bcf86cd799439027";

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = "507f1f77bcf86cd799439028",
            RequestId = requestId,
            Description = "Request activity 1",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439029",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new Activity
        {
            AssignmentId = "507f1f77bcf86cd79943902a",
            RequestId = requestId,
            Description = "Request activity 2",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd79943902b",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Act
        var result = await _repository.GetByRequestIdAsync(requestId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.RequestId == requestId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateActivity()
    {
        // Arrange
        var activity = new Activity
        {
            AssignmentId = "507f1f77bcf86cd79943902c",
            RequestId = "507f1f77bcf86cd79943902d",
            Description = "Original description",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd79943902e",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _repository.CreateAsync(activity);

        // Act
        created.Description = "Updated description";
        created.IsEditable = false;
        var updated = await _repository.UpdateAsync(created.Id, created);

        // Assert
        updated.Should().BeTrue();

        var result = await _repository.GetByIdAsync(created.Id);
        result.Description.Should().Be("Updated description");
        result.IsEditable.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteActivity()
    {
        // Arrange
        var activity = new Activity
        {
            AssignmentId = "507f1f77bcf86cd79943902f",
            RequestId = "507f1f77bcf86cd799439030",
            Description = "To be deleted",
            ActivityDate = DateTime.UtcNow,
            SubmittedByUserId = "507f1f77bcf86cd799439031",
            IsEditable = true,
            Files = new List<ActivityFile>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _repository.CreateAsync(activity);

        // Act
        var deleted = await _repository.DeleteAsync(created.Id);

        // Assert
        deleted.Should().BeTrue();

        var result = await _repository.GetByIdAsync(created.Id);
        result.Should().BeNull();
    }
}