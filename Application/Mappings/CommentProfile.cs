using AutoMapper;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<UpdateCommentDto, Comment>();
        }
    }
}

