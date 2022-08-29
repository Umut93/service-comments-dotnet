using Microsoft.EntityFrameworkCore;
using Unik.Comments.Infrastructure.Models;
using Unik.SaaS.Infrastructure.EntityFramework.InstanceIsolation;
using Unik.SaaS.Infrastructure.EntityFramework.TenantIsolation;

namespace Unik.Comments.Infrastructure.Contexts;

public sealed class CommentsDbContext : InstanceDbContextBase
{
    public CommentsDbContext(
        IInstanceContext instanceContext,
        ITenantContext tenantContext,
        DbContextOptions dbContextOptions)
        : base(instanceContext, tenantContext, dbContextOptions)
    {
    }

    public DbSet<CommentEntity> Comments { get; set; } = null!;

    protected override void OnDatabaseModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommentsDbContext).Assembly);
    }
}