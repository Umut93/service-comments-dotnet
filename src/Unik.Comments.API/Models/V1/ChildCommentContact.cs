namespace Unik.Comments.API.Models.V1;

/// <summary>
/// Comment contract model
/// </summary>
public class ChildCommentContact : CommentContract
{
    /// <summary>
    /// Nummer of replies to the comment
    /// </summary>
    public int DescendantCommentsCount { get; set; }
}