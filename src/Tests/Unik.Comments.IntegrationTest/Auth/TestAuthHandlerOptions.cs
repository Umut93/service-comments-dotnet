using Microsoft.AspNetCore.Authentication;

namespace Unik.Comments.IntegrationTest.Auth;

/// <summary>
/// AuthenticationSchemeOptions will validate the different options provided by the scheme it will be used by AuthenticationHandler  <see cref="TestAuthHandler"/>
/// </summary>
public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultTenantId { get; set; } = "af620db7-4b2e-4ed6-8c90-7b76d622f8bd";
    public string DefaultInstanceId { get; set; } = "39a8eeed-9a7e-441b-8eb0-4c1810f56dd8";
    public string DefaultUserId { get; set; } = "1d2ec7f0-6601-467a-9ae8-042d0eb9c652";
}