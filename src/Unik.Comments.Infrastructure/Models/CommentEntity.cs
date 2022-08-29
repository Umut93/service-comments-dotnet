using System;
using System.Collections.Generic;

namespace Unik.Comments.Infrastructure.Models;

public class CommentEntity
{
    public int InternalId { get; set; }

    public Guid PublicId { get; set; }

    public int ThreadId { get; set; }

    public CommentContentEntity? Content { get; set; }

    public int? ParentId { get; set; }

    public CommentEntity? Parent { get; set; }

    public ICollection<CommentEntity> Replies { get; set; } = new List<CommentEntity>();

    public Guid CreatedBy { get; set; }

    public DateTimeOffset Created { get; set; }
}