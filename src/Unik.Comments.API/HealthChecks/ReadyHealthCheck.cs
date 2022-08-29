using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace Unik.Comments.API.HealthChecks;
/// <summary>
/// The class tells if the microservice is ready. 
/// The readiness indicates if the app is running normally but isn't ready to receive requests.
/// </summary>
public class ReadyHealthCheck : IHealthCheck
{
    public bool IsReady { get; set; } = false;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (IsReady)
        {
            return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));
        }
        return Task.FromResult(HealthCheckResult.Unhealthy("That startup task is still running."));
    }
}
