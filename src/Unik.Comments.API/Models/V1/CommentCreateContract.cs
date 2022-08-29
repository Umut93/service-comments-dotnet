using System;

namespace Unik.Comments.API.Models.V1;

/// <summary>
/// Comment create model
/// </summary>
public class CommentCreateContract
{
    /// <summary>
    /// the comment content to create
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Id of the parent comment
    /// </summary>
    public Guid? ParentId { get; set; }
}