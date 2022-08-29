using System;

namespace Unik.Comments.API.Models.V1;

/// <summary>
/// Comment update model
/// </summary>
public class CommentUpdateContract
{
    /// <summary>
    /// Id of the comment to update
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// the new comment content
    /// </summary>
    public string? Data { get; set; }
}