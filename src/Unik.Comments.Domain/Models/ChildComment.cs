namespace Unik.Comments.Domain.Models;

public class ChildComment : Comment
{
    public int DescendantCommentsCount { get; set; }
}