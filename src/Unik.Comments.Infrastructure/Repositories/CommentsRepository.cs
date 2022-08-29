using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
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
}