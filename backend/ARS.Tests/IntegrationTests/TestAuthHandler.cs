using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ARS.Tests.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "test-user-id-123"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test@example.com"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "Test"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "User"),
            new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "PMO"),
            new Claim("name", "Test User")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}