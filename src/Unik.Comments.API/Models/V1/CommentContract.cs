using System;
using System.Collections.Generic;

namespace Unik.Comments.API.Models.V1;

/// <summary>
/// Comment contract model
/// </summary>
public class CommentContract
{
    /// <summary>
    /// Id of the comment
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// the comment content
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// Id of the parent comment
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Collection of replies to the comment
    /// </summary>
    public ICollection<CommentContract> Replies { get; set; } = new List<CommentContract>();

    /// <summary>
    /// The id of the user who created the comment
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Time of when the comment was created
    /// </summary>
    public DateTimeOffset Created { get; set; }
}