using System;

namespace Unik.Comments.Infrastructure.Models;

public class CommentContentEntity
{
    private CommentEntity _comment = null!;

    public int Id { get; set; }

    public string? Data { get; set; }

    public int CommentId { get; set; }

    public CommentEntity Comment { get => _comment!; set => _comment = value ?? throw new ArgumentNullException(nameof(Comment)); }
}