using AutoMapper;
using System;
using Unik.Comments.Domain.Models;

namespace Unik.Comments.API.Models.V1.Mappers;

/// <summary>
/// Mappers for Comments
/// </summary>
public class CommentMappers : Profile
{
    /// <summary>
    /// Specified mappers to and from the comment contract models
    /// </summary>
    public CommentMappers()
    {
        CreateMap<CommentCreateContract, Comment>()
            .BeforeMap((src, dest) => dest.PublicId = Guid.NewGuid())
            .BeforeMap((src, dest) => dest.Created = DateTime.Now)
            .BeforeMap((src, dest) => dest.Content = new CommentContent())
            .ForMember(dest => dest.ParentPublicId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.ParentId, opt => opt.Ignore())
            .ForPath(dest => dest.Content!.Data, opt => opt.MapFrom(src => src.Content));

        CreateMap<Comment, CommentContract>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentPublicId))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Content!.Data));


        CreateMap<ChildComment, ChildCommentContact>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentPublicId))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Content!.Data));

    }
}