using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unik.Comments.API.HealthChecks;
public class LiveHealthCheck : IHealthCheck
{
    private readonly ILogger<LiveHealthCheck> logger;

    public LiveHealthCheck(ILogger<LiveHealthCheck> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = false;

        this.logger.LogDebug("CheckHealthAsync is executed...");
        // TODO: Implement the liveness health probe.

        isHealthy = true;

        return (isHealthy ?
            Task.FromResult(HealthCheckResult.Healthy("A healthy result."))
            : Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result.")));
    }
}
