using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unik.Comments.Infrastructure.Models;

namespace Unik.Comments.Infrastructure.Configuration;

public class CommentContentConfiguration : IEntityTypeConfiguration<CommentContentEntity>
{
    public void Configure(EntityTypeBuilder<CommentContentEntity> builder)
    {
        builder.ToTable("CommentsContent")
            .HasKey(x => x.Id);

        builder.Property(x => x.Data)
            .HasColumnType("nvarchar(max)")
            .IsRequired();
    }
}