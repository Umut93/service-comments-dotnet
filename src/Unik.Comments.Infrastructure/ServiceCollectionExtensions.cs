using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unik.Comments.Domain.Repositories;
using Unik.Comments.Infrastructure.Contexts;
using Unik.Comments.Infrastructure.Repositories;
using Unik.SaaS.Infrastructure.EntityFramework;

namespace Unik.Comments.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICommentsRepository, CommentsRepository>();
        services.AddDbContext<CommentsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("Unik.Comments.Infrastructure"));
        });

        services.AddInstanceIsolation();
        services.AddTenantIsolation();
        return services;
    }
}