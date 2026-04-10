using ARS.Domain.Entities;
using ARS.Domain.Enums;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;

namespace ARS.Tests.IntegrationTests.ApiTests;

public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        // Configure JSON to handle enums as strings
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [Fact]
    public async Task SyncCurrentUser_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Act
        var response = await _client.PostAsync("/api/users/sync-current-user", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(json, _jsonOptions);

        user.Should().NotBeNull();
        user!.Email.Should().Be("test@example.com");
        user.FirstName.Should().Be("Test");
        user.LastName.Should().Be("User");
        user.Role.Should().Be(UserRole.PMO);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers()
    {
        // Arrange - Create a user first
        await _client.PostAsync("/api/users/sync-current-user", null);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<User>>(json, _jsonOptions);

        users.Should().NotBeNull();
        users.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange - Create a user
        var syncResponse = await _client.PostAsync("/api/users/sync-current-user", null);
        var syncJson = await syncResponse.Content.ReadAsStringAsync();
        var createdUser = JsonSerializer.Deserialize<User>(syncJson, _jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/users/{createdUser!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(json, _jsonOptions);

        user.Should().NotBeNull();
        user!.Id.Should().Be(createdUser.Id);
        user.Email.Should().Be("test@example.com");
        user.Role.Should().Be(UserRole.PMO);
    }

    [Fact]
    public async Task GetUserById_ShouldReturn404_WhenUserDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/users/507f1f77bcf86cd799439011");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}