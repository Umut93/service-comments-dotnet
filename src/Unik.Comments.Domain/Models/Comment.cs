using System;
using System.Collections.Generic;

namespace Unik.Comments.Domain.Models;

public class Comment
{
    public int InternalId { get; set; }
    public Guid PublicId { get; set; }
    public int ThreadId { get; set; }
    public CommentContent? Content { get; set; }
    public int? ParentId { get; set; }
    public Guid? ParentPublicId { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public Guid CreatedBy { get; set; }
    public DateTimeOffset Created { get; set; }
}