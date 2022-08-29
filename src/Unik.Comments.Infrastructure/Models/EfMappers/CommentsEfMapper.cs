using AutoMapper;
using Unik.Comments.Domain.Models;

namespace Unik.Comments.Infrastructure.Models.EfMappers;

public class CommentsEfMapper : Profile
{
    public CommentsEfMapper()
    {
        CreateMap<CommentEntity, Comment>();
        CreateMap<Comment, CommentEntity>();
        CreateMap<CommentContent, CommentContentEntity>();
        CreateMap<CommentContentEntity, CommentContent>();
    }
}