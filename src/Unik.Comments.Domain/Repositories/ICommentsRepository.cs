using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unik.Comments.Domain.Models;

namespace Unik.Comments.Domain.Repositories;

public interface ICommentsRepository
{
    Task<Comment> CreateCommentAsync(Comment comment);

    Task<Comment?> GetCommentByIdAsync(Guid id);

    Task<Guid> GetCommentPublicIdAsync(int id);

    Task<int> GetCommentInternalIdAsync(Guid id);

    Task<bool> UpdateCommentAsync(Comment comment);

    Task<bool> DeleteCommentAsync(Comment comment);

    Task<ICollection<ChildComment>> GetChildCommentsAsync(Guid id, int offset, int limit);
}