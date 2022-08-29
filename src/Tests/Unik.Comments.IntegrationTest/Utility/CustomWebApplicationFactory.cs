using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Unik.Comments.Infrastructure.Contexts;
using Unik.Comments.IntegrationTest.Auth;

namespace Unik.Comments.IntegrationTest.Utility;

/// <summary>
/// Customize WebApplicationFactory - https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0#customize-webapplicationfactory
/// The intension of customizing webapplicationfactory is putting the factory for bootstrapping an application in memory for functional end to end tests
/// </summary>
/// <typeparam name="TStartup"></typeparam>
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
#if DEBUG
        var dockerSqlPort = DockerDatabaseInitializer.EnsureDockerStartedAndGetContainerIdAndPortAsync().Result;
#else
        const string dockerSqlPort = "1433";
#endif

        var dockerConnectionString = DockerDatabaseInitializer.GetSqlConnectionString(dockerSqlPort, DockerDatabaseInitializer.DB_DATABASE);

        builder.ConfigureServices(services =>
        {
            // Add authentication policy Test
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Test")
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthentication("Test")
                // Adding test scheme which we authenticate against instead of the default bearer scheme
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("Test", _ => { });

            var descriptor = services.Single(
                service => service.ServiceType == typeof(DbContextOptions<CommentsDbContext>));

            services.Remove(descriptor);

            services.AddDbContext<CommentsDbContext>(options => options.UseSqlServer(dockerConnectionString), ServiceLifetime.Singleton);
        });

        base.ConfigureWebHost(builder);
    }
}