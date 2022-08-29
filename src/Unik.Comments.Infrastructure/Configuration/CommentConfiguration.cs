using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unik.Comments.Infrastructure.Models;

namespace Unik.Comments.Infrastructure.Configuration;

public class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.ToTable("Comments")
            .HasKey(x => x.InternalId);

        builder.Property(x => x.PublicId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(x => x.ThreadId)
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne(x => x.Content)
            .WithOne(x => x.Comment)
            .HasForeignKey<CommentContentEntity>(x => x.CommentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Replies)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => x.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedBy)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(x => x.Created)
            .HasColumnType("datetimeoffset")
            .IsRequired();
    }
}