using System;
using System.Threading.Tasks;
using Unik.Comments.Domain.Models;

namespace Unik.Comments.Domain.Services;

public interface ICommentsService
{
    Task<Comment> CreateCommentAsync(Comment comment);

    Task<Comment?> GetCommentByIdAsync(Guid id);

    Task<bool> UpdateCommentAsync(Guid publicId, string data);

    Task<bool> DeleteCommentAsync(Guid id);
}