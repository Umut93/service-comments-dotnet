using Microsoft.Extensions.DependencyInjection;
using Unik.Comments.Domain.Services;

namespace Unik.Comments.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ICommentsService, CommentsService>();

        return services;
    }
}