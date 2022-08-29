using System;
using System.Threading.Tasks;
using Unik.Comments.Domain.Models;
using Unik.Comments.Domain.Repositories;

namespace Unik.Comments.Domain.Services;

public class CommentsService : ICommentsService
{
    private readonly ICommentsRepository _commentsRepository;

    public CommentsService(ICommentsRepository commentsRepository)
    {
        _commentsRepository = commentsRepository;
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        if (comment.ParentPublicId is not null)
        {
            var parent = await _commentsRepository.GetCommentByIdAsync(comment.ParentPublicId.Value);

            ArgumentNullException.ThrowIfNull(parent, nameof(parent));

            comment.ParentId = parent.InternalId;
            comment.ThreadId = parent.ThreadId;
        }

        var createdComment = await _commentsRepository.CreateCommentAsync(comment);

        if (createdComment.ParentId is not null)
        {
            createdComment.ParentPublicId = comment.ParentPublicId;
        }

        return createdComment;
    }

    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        var comment = await _commentsRepository.GetCommentByIdAsync(id);

        if (comment is null)
        {
            return false;
        }

        var commentToDelete = await AddReplies(comment);

        return await _commentsRepository.DeleteCommentAsync(commentToDelete);
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _commentsRepository.GetCommentByIdAsync(id);
        if (comment is null)
        {
            return null;
        }

        await AddReplies(comment);

        if (comment.ParentId is not null)
        {
            comment.ParentPublicId = await _commentsRepository.GetCommentPublicIdAsync(comment.ParentId.Value);
        }

        return comment;
    }

    private async Task<Comment> AddReplies(Comment comment)
    {
        foreach (var reply in comment.Replies)
        {
            var fullReply = await _commentsRepository.GetCommentByIdAsync(reply.PublicId);
            if (fullReply is not null)
            {
                reply.Replies = fullReply.Replies;
                reply.ParentPublicId = comment.PublicId;
            }
            await AddReplies(reply);
        }

        return comment;
    }

    public async Task<bool> UpdateCommentAsync(Guid publicId, string data)
    {
        var comment = await _commentsRepository.GetCommentByIdAsync(publicId);
        if (comment is null || comment.Content is null)
        {
            return false;
        }

        comment.Content.Data = data;

        return await _commentsRepository.UpdateCommentAsync(comment);
    }
}