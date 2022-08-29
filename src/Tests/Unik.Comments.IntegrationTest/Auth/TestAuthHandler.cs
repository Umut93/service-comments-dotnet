using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Unik.Comments.IntegrationTest.Auth;

/// <summary>
/// See Integration test on .NET 6.0 https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
/// The test app can mock an AuthenticationHandler<TOptions> in ConfigureTestServices in order to test aspects of authentication and authorization.
/// A minimal scenario returns an AuthenticateResult.Success:
/// </summary>
internal sealed class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    private readonly string _defaultTenantId;
    private readonly string _defaultInstanceId;
    private readonly string _defaultUserId;
    private readonly AuthenticationTicket _ticket;

    public TestAuthHandler(
        IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _defaultTenantId = options.CurrentValue.DefaultTenantId;
        _defaultInstanceId = options.CurrentValue.DefaultInstanceId;
        _defaultUserId = options.CurrentValue.DefaultUserId;

        var claims = new List<Claim>
        {
            new Claim("tenantId", _defaultTenantId),
            new Claim("instanceId", _defaultInstanceId),
            new Claim("userId", _defaultUserId),
            new Claim("email", Guid.NewGuid().ToString()),
            new Claim("name", Guid.NewGuid().ToString()),
            new Claim("sub", "a11dc215-b8d1-48b1-bbdc-d9ad48abce39"),
            new Claim("aud", "86e644c3-7423-4ff1-89b5-b44c2ea76563"),
            new Claim("ccr", "B2C_1A_SIGNIN")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _ticket = new AuthenticationTicket(principal, "Test");
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(AuthenticateResult.Success(_ticket));
}