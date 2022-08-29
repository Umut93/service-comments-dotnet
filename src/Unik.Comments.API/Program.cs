using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Serilog;
using System;
using Unik.Comments.API.HealthChecks;
using Unik.Comments.Domain;
using Unik.Comments.Infrastructure;
using Unik.Comments.Infrastructure.Contexts;
using Unik.SaaS.Infrastructure.EntityFramework;
using Unik.Swagger;

var builder = WebApplication.CreateBuilder(args);

#region Setup logging

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Seq(serverUrl: "http://test.log:5341")
    .WriteTo.Console()
    .CreateLogger();
Log.Information("Application starting");

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

#endregion Setup logging

// Add services to the container.
builder.Services.AddSingleton<ReadyHealthCheck>();

// configure services with a http client:
builder.Services.AddDomain()
                .AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorizationMiddleware();

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureUnikSwaggerServices();

builder.Services.AddHealthChecks()
    .AddCheck<ReadyHealthCheck>("Ready", HealthStatus.Unhealthy, tags: new[] { "Ready" })
    .AddCheck<LiveHealthCheck>("Live", HealthStatus.Unhealthy, tags: new[] { "Live" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    using (var context = scope.ServiceProvider.GetRequiredService<CommentsDbContext>())
    {
        context.Database.Migrate();
    }
}

app.UseUnikSwagger(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseInstanceContextMiddleware();
app.UseTenantContextMiddleware();

app.MapControllers();

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    // Predicate = healthCheck => healthCheck.Tags.Contains("Live")
    Predicate = _ => true // this will call all health checks (including readiness).
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("Ready")
});

// set the ready flag when the service is started and ready to receive requests
var readyHealthCheck = app.Services.GetService<ReadyHealthCheck>();
if (readyHealthCheck != null)
{
    readyHealthCheck.IsReady = true;
}

app.Run();

// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
public partial class Program
{ }