using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Data;
using ARS.Infrastructure.Repositories;
using ARS.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace ARS.Tests.UnitTests.Repositories;

public class UserRepositoryTests : TestBase
{
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new User
        {
            EntraObjectId = "test-oid-123",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.Team,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("Test");
        result.Role.Should().Be(UserRole.Team);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            EntraObjectId = "test-oid-456",
            Email = "existing@example.com",
            FirstName = "Existing",
            LastName = "User",
            Role = UserRole.PMO,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(created.Id);
        result.Email.Should().Be("existing@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync("507f1f77bcf86cd799439011");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Arrange
        var user = new User
        {
            EntraObjectId = "test-oid-789",
            Email = "unique@example.com",
            FirstName = "Unique",
            LastName = "User",
            Role = UserRole.Entity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByEmailAsync("unique@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("unique@example.com");
    }

    [Fact]
    public async Task GetByEntraIdAsync_ShouldReturnUser_WhenEntraIdExists()
    {
        // Arrange
        var entraId = "azure-oid-abc123";
        var user = new User
        {
            EntraObjectId = entraId,
            Email = "azure@example.com",
            FirstName = "Azure",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByEntraIdAsync(entraId);

        // Assert
        result.Should().NotBeNull();
        result.EntraObjectId.Should().Be(entraId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = new User
        {
            EntraObjectId = "update-test-123",
            Email = "original@example.com",
            FirstName = "Original",
            LastName = "User",
            Role = UserRole.Team,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _repository.CreateAsync(user);

        // Act
        created.Email = "updated@example.com";
        created.FirstName = "Updated";
        created.Role = UserRole.PMO;
        var updated = await _repository.UpdateAsync(created.Id, created);

        // Assert
        updated.Should().BeTrue();

        var result = await _repository.GetByIdAsync(created.Id);
        result.Email.Should().Be("updated@example.com");
        result.FirstName.Should().Be("Updated");
        result.Role.Should().Be(UserRole.PMO);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
    {
        // Arrange
        var user = new User
        {
            EntraObjectId = "delete-test-123",
            Email = "delete@example.com",
            FirstName = "Delete",
            LastName = "User",
            Role = UserRole.Team,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _repository.CreateAsync(user);

        // Act
        var deleted = await _repository.DeleteAsync(created.Id);

        // Assert
        deleted.Should().BeTrue();

        var result = await _repository.GetByIdAsync(created.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByRoleAsync_ShouldReturnUsersWithRole()
    {
        // Arrange
        await _repository.CreateAsync(new User
        {
            EntraObjectId = "pmo1",
            Email = "pmo1@example.com",
            FirstName = "PMO",
            LastName = "One",
            Role = UserRole.PMO,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new User
        {
            EntraObjectId = "pmo2",
            Email = "pmo2@example.com",
            FirstName = "PMO",
            LastName = "Two",
            Role = UserRole.PMO,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new User
        {
            EntraObjectId = "team1",
            Email = "team1@example.com",
            FirstName = "Team",
            LastName = "Member",
            Role = UserRole.Team,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Act
        var pmoUsers = await _repository.GetByRoleAsync("PMO");

        // Assert
        pmoUsers.Should().HaveCount(2);
        pmoUsers.Should().OnlyContain(u => u.Role == UserRole.PMO);
    }

    [Fact]
    public async Task GetActiveUsersAsync_ShouldReturnAllActiveUsers()
    {
        // Arrange
        await _repository.CreateAsync(new User
        {
            EntraObjectId = "active1",
            Email = "active1@example.com",
            FirstName = "Active",
            LastName = "One",
            Role = UserRole.Team,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _repository.CreateAsync(new User
        {
            EntraObjectId = "inactive1",
            Email = "inactive1@example.com",
            FirstName = "Inactive",
            LastName = "One",
            Role = UserRole.Team,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Act
        var activeUsers = await _repository.GetActiveUsersAsync();

        // Assert
        activeUsers.Should().HaveCount(1);
        activeUsers.Should().OnlyContain(u => u.IsActive);
    }
}