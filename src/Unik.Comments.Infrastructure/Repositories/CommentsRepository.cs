using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unik.Comments.Domain.Exceptions;
using Unik.Comments.Domain.Models;
using Unik.Comments.Domain.Repositories;
using Unik.Comments.Infrastructure.Contexts;
using Unik.Comments.Infrastructure.Models;

namespace Unik.Comments.Infrastructure.Repositories;

public class CommentsRepository : ICommentsRepository
{
    private readonly IMapper _mapper;
    private readonly CommentsDbContext _context;

    public CommentsRepository(IMapper mapper, CommentsDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        var commentEf = _mapper.Map<CommentEntity>(comment);
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            await _context.Comments.AddAsync(commentEf);
            await _context.SaveChangesAsync();

            if (commentEf.ParentId is null)
            {
                commentEf.ThreadId = commentEf.InternalId;
                await _context.SaveChangesAsync();
            }

            transaction.Commit();
            return _mapper.Map<Comment>(commentEf);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new DataAccessException(e.Message, e.InnerException);
        }
    }

    public async Task<bool> DeleteCommentAsync(Comment comment)
    {
        var commentEf = _mapper.Map<CommentEntity>(comment);
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Comments.Remove(commentEf);
            var success = (await _context.SaveChangesAsync()) > 0;
            transaction.Commit();

            return success;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new DataAccessException(e.Message, e.InnerException);
        }
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        var result = await _context.Comments
            .AsNoTracking()
            .Include(comment => comment.Content)
            .Include(comment => comment.Replies)
            .ThenInclude(comment => comment.Content)
            .FirstOrDefaultAsync(comment => comment.PublicId == id);
        return result is not null ? _mapper.Map<Comment>(result) : null;
    }

    public async Task<Guid> GetCommentPublicIdAsync(int id)
    {
        return await _context.Comments
            .Where(comment => comment.InternalId == id)
            .Select(comment => comment.PublicId)
            .SingleOrDefaultAsync();
    }

    public async Task<int> GetCommentInternalIdAsync(Guid id)
    {
        return await _context.Comments
            .Where(comment => comment.PublicId == id)
            .Select(comment => comment.InternalId)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> UpdateCommentAsync(Comment comment)
    {
        var commentEf = _mapper.Map<CommentEntity>(comment);
        _context.Update<CommentEntity>(commentEf);
        return (await _context.SaveChangesAsync()) > 0;
    }

    /// <summary>
    /// From a given root node it displays all the descandants nodes and also for each descandant node it displays how many descandants nodes there are further.
    /// Root = depth level 0
    /// Descandant nodes from root = level 1
    /// Descandant nodes from first level = level 2
    /// </summary>
    /// <param name="id"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<ICollection<ChildComment>> GetChildCommentsAsync(Guid id, int offset, int limit)
    {
        ICollection<ChildComment> firstDepthComments = new List<ChildComment>();

        var result = await _context.Comments
           .AsNoTracking()
           .Include(comment => comment.Content)
           .Include(depthOneComment => depthOneComment.Replies.Skip(offset).Take(limit)).ThenInclude(secondDepthComment => secondDepthComment.Replies)
           .ThenInclude(comment => comment.Content)
           .FirstOrDefaultAsync(comment => comment.PublicId == id);

        foreach (var replyComment in result?.Replies ?? Array.Empty<CommentEntity>())
        {
            var childComment = _mapper.Map<ChildComment>(replyComment);
            childComment.DescendantCommentsCount = replyComment.Replies.Count;
            firstDepthComments.Add(childComment);
        }

        return firstDepthComments;
    }
}